using DevsWhoRun.Api;
using DevsWhoRun.Api.Modules.Auth.DTOs;
using DevsWhoRun.Api.Modules.Auth.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace DevsWhoRun.IntegrationTests.Modules.Auth.Services;

public class AuthServiceTests : IDisposable
{
    private readonly DbContextOptions<DevsWhoRunDbContext> _dbContextOptions;
    private readonly IConfiguration _configurationMock;
    private readonly HttpClient _httpClientMock;

    public AuthServiceTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<DevsWhoRunDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _configurationMock = Substitute.For<IConfiguration>();
        _httpClientMock = Substitute.For<HttpClient>();
        _configurationMock["Jwt:Key"] = "this is veryveryveryverysecrestkeyfortesting";
    }

    public void Dispose()
    {
        using var dbContext = new DevsWhoRunDbContext(_dbContextOptions);
        dbContext.Database.EnsureDeleted();
    }

    [Fact]
    public async Task Register_ReturnsAuthResponse_WhenRegistrationIsSuccessful()
    {
        await using var dbContext = new DevsWhoRunDbContext(_dbContextOptions);
        var authService = new AuthService(_configurationMock, dbContext, _httpClientMock);

        var request = new RegisterRequest(
            "John", 
            "Doe", 
            "john.doe@example.com", 
            "SecurePassword123", 
            "johnGitHub", 
            "stravaProfile");

        var response = await authService.RegisterAsync(request);

        response.Should().NotBeNull();
        response.Token.Should().NotBeNullOrEmpty();
        response.RefreshToken.Should().NotBeNullOrEmpty();
        response.Member.Email.Should().Be(request.Email);
    }


    [Fact]
    public async Task Register_ThrowsException_WhenEmailAlreadyRegistered()
    {
        // Arrange
        await using var dbContext = new DevsWhoRunDbContext(_dbContextOptions);
        var authService = new AuthService(_configurationMock, dbContext, _httpClientMock);

        // Add existing member with all required fields
        dbContext.Members.Add(new Member
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("SecurePassword123"),
            GitHubUserName = "janeGitHub", // Populate required field
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            LastLogin = DateTime.UtcNow,
            UserType = UserType.User
        });
        await dbContext.SaveChangesAsync();

        var request = new RegisterRequest("Jane", "Doe", "jane.doe@example.com", "SecurePassword123", "janeGitHub", "stravaProfile");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => authService.RegisterAsync(request));
        exception.Message.Should().Be("Email already registered");
    }


    [Fact]
    public async Task Login_ReturnsAuthResponse_WhenLoginIsSuccessful()
    {
        await using var dbContext = new DevsWhoRunDbContext(_dbContextOptions);
        var authService = new AuthService(_configurationMock, dbContext, _httpClientMock);

        dbContext.Members.Add(new Member
        {
            FirstName = "Test",
            LastName = "User",
            Email = "user@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("SecurePassword123"),
            GitHubUserName = "testGitHub",
            StravaProfile = "stravaProfile"
        });
        await dbContext.SaveChangesAsync();

        var request = new LoginRequest("user@example.com", "SecurePassword123");
        var response = await authService.LoginAsync(request);

        response.Should().NotBeNull();
        response.Token.Should().NotBeNullOrEmpty();
        response.RefreshToken.Should().NotBeNullOrEmpty();
        response.Member.Email.Should().Be("user@example.com");
    }

    [Fact]
    public async Task RefreshToken_ReturnsAuthResponse_WhenTokenIsValid()
    {
        await using var dbContext = new DevsWhoRunDbContext(_dbContextOptions);
        _configurationMock["Jwt:Key"] = "this is veryveryveryverysecrestkeyfortesting";
        var authService = new AuthService(_configurationMock, dbContext, _httpClientMock);

        var refreshToken = "validRefreshToken";
        dbContext.Members.Add(new Member
        {
            FirstName = "Test",
            LastName = "User",
            Email = "user@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("SecurePassword123"),
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1),
            GitHubUserName = "testGitHub",
            StravaProfile = "stravaProfile"
        });
        await dbContext.SaveChangesAsync();

        var response = await authService.RefreshTokenAsync(refreshToken);

        response.Should().NotBeNull();
        response.Token.Should().NotBeNullOrEmpty();
        response.RefreshToken.Should().NotBe(refreshToken);
    }
    
    private static Member GivenMember()
    {
        return new Member
        {
            FirstName = "Test",
            LastName = "User",
            Email = "user@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("SecurePassword123"),
            UserType = UserType.User,
            GitHubUserName = "testGitHub",
            StravaProfile = "stravaProfile"
        };
    }
}
