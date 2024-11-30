using devs_who_run_api.Modules.Auth.DTOs;

namespace devs_who_run_api.Modules.Auth.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);
    Task<AuthResponse> GitHubLoginAsync(string code);
}
