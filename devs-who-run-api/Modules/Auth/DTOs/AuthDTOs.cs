using System.ComponentModel.DataAnnotations;

namespace devs_who_run_api.Modules.Auth.DTOs;

public record LoginRequest(
    [Required][EmailAddress] string Email,
    [Required] string Password
);

public record RegisterRequest(
    [Required] string FirstName,
    [Required] string LastName,
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)] string Password,
    string? GitHubUserName,
    string? StravaProfile
);

public record AuthResponse(
    string Token,
    string RefreshToken,
    DateTime ExpiresAt,
    MemberDTO Member
);

public record MemberDTO(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string? GitHubUserName,
    string? StravaProfile,
    string? Avatar,
    UserType UserType
);

public record RefreshTokenRequest(
    [Required] string RefreshToken
);

public record GitHubAuthRequest(
    [Required] string Code
);
