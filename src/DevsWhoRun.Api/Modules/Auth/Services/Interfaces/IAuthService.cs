using DevsWhoRun.Api.Modules.Auth.DTOs;

namespace DevsWhoRun.Api.Modules.Auth.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);
    Task<AuthResponse> GitHubLoginAsync(string code);
}
