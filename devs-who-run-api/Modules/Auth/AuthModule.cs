using Carter;
using devs_who_run_api.Modules.Auth.DTOs;
using devs_who_run_api.Modules.Auth.Services.Interfaces;

namespace devs_who_run_api.Modules.Auth;

public class AuthModule : CarterModule
{
    public AuthModule() : base("/api/auth")
    {
        WithTags("Auth");
        IncludeInOpenApi();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/register", async (RegisterRequest request, IAuthService authService) =>
        {
            try
            {
                var response = await authService.RegisterAsync(request);
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        })
        .WithName("Register");

        app.MapPost("/login", async (LoginRequest request, IAuthService authService) =>
        {
            try
            {
                var response = await authService.LoginAsync(request);
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        })
        .WithName("Login");

        app.MapPost("/refresh-token", async (RefreshTokenRequest request, IAuthService authService) =>
            {
                try
                {
                    var response = await authService.RefreshTokenAsync(request.RefreshToken);
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
            })
            .WithName("RefreshToken");

        app.MapGet("/github-callback", async (string code, IAuthService authService) =>
            {
                try
                {
                    var response = await authService.GitHubLoginAsync(code);
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
            })
            .WithName("GitHubCallback");
    }
}