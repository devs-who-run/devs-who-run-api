using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DevsWhoRun.Api.Modules.Auth.DTOs;
using DevsWhoRun.Api.Modules.Auth.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DevsWhoRun.Api.Modules.Auth.Services;

public class AuthService(IConfiguration configuration, DevsWhoRunDbContext context, HttpClient httpClient)
    : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request), "Request cannot be null");
        }

        var existingMember = await context.Members.FirstOrDefaultAsync(m => m.Email == request.Email);
        if (existingMember != null)
        {
            throw new Exception("Email already registered");
        }

        var member = new Member
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            GitHubUserName = request.GitHubUserName ?? "",
            StravaProfile = request.StravaProfile,
            UserType = UserType.User,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            LastLogin = DateTime.UtcNow,
            IsEmailVerified = false
        };

        context.Members.Add(member);
        await context.SaveChangesAsync();

        var (token, refreshToken, expiresAt) = GenerateTokens(member);
        member.RefreshToken = refreshToken;
        member.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await context.SaveChangesAsync();

        return new AuthResponse(
            token,
            refreshToken,
            expiresAt,
            MapToMemberDto(member)
        );
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var member = await context.Members
            .FirstOrDefaultAsync(m => m.Email != null && m.Email.ToLower() == request.Email.ToLower());
        if (member?.PasswordHash == null || !BCrypt.Net.BCrypt.Verify(request.Password, member.PasswordHash))
        {
            throw new Exception("Invalid credentials");
        }

        var (token, refreshToken, expiresAt) = GenerateTokens(member);
        member.RefreshToken = refreshToken;
        member.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        member.LastLogin = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return new AuthResponse(
            token,
            refreshToken,
            expiresAt,
            MapToMemberDto(member)
        );
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var member = await context.Members.FirstOrDefaultAsync(m => m.RefreshToken == refreshToken);
        if (member == null || member.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new Exception("Invalid or expired refresh token");
        }

        var (newToken, newRefreshToken, expiresAt) = GenerateTokens(member);
        member.RefreshToken = newRefreshToken;
        member.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await context.SaveChangesAsync();

        return new AuthResponse(
            newToken,
            newRefreshToken,
            expiresAt,
            MapToMemberDto(member)
        );
    }

    public async Task<AuthResponse> GitHubLoginAsync(string code)
    {
        var githubToken = await GetGitHubAccessTokenAsync(code);
        var githubUser = await GetGitHubUserAsync(githubToken);

        var member = await context.Members.FirstOrDefaultAsync(m => m.GithubUserId == githubUser.Id);
        if (member == null)
        {
            // Create new member
            member = new Member
            {
                FirstName = githubUser.Name?.Split(' ').FirstOrDefault() ?? githubUser.Login,
                LastName = githubUser.Name?.Split(' ').Skip(1).FirstOrDefault() ?? "",
                Email = githubUser.Email ?? $"{githubUser.Login}@github.com",
                GitHubUserName = githubUser.Login,
                GithubUserId = githubUser.Id,
                Avatar = githubUser.AvatarUrl,
                UserType = UserType.User,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                IsEmailVerified = true
            };
            context.Members.Add(member);
        }
        else
        {
            member.LastLogin = DateTime.UtcNow;
            member.Avatar = githubUser.AvatarUrl;
        }

        var (token, refreshToken, expiresAt) = GenerateTokens(member);
        member.RefreshToken = refreshToken;
        member.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await context.SaveChangesAsync();

        return new AuthResponse(
            token,
            refreshToken,
            expiresAt,
            MapToMemberDto(member)
        );
    }

    private (string token, string refreshToken, DateTime expiresAt) GenerateTokens(Member member)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, member.Id.ToString()),
            new(ClaimTypes.Email, member.Email!),
            new(ClaimTypes.Role, member.UserType.ToString()),
            new(ClaimTypes.GivenName, member.FirstName!),
            new(ClaimTypes.Surname, member.LastName!)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddHours(1);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        return (new JwtSecurityTokenHandler().WriteToken(token), refreshToken, expiresAt);
    }

    private async Task<string> GetGitHubAccessTokenAsync(string code)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
        var parameters = new Dictionary<string, string>
        {
            {"client_id", configuration["GitHub:ClientId"]!},
            {"client_secret", configuration["GitHub:ClientSecret"]!},
            {"code", code}
        };

        request.Content = new FormUrlEncodedContent(parameters);
        request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GitHubTokenResponse>();
        return result?.AccessToken ?? throw new Exception("Failed to get GitHub access token");
    }

    private async Task<GitHubUser> GetGitHubUserAsync(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("DevsWhoRun", "1.0"));

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<GitHubUser>() 
               ?? throw new Exception("Failed to get GitHub user data");
    }

    private static MemberDTO MapToMemberDto(Member member) => new(
        member.Id,
        member.FirstName!,
        member.LastName!,
        member.Email!,
        member.GitHubUserName,
        member.StravaProfile,
        member.Avatar,
        member.UserType
    );
}

// Helper classes for GitHub API responses
public class GitHubTokenResponse
{
    public string? AccessToken { get; set; }
    public string? TokenType { get; set; }
    public string? Scope { get; set; }
}

public class GitHubUser
{
    public string? Login { get; set; }
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? AvatarUrl { get; set; }
}
