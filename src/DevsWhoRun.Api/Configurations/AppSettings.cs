using System.Security.Claims;

namespace DevsWhoRun.Api.Configurations;

public class AppSettings
{
    public DatabaseSettings Database { get; set; } = new();
    public SwaggerSettings Swagger { get; set; } = new();
    public JwtSettings Jwt { get; set; } = new();
    public GitHubSettings GitHub { get; set; } = new();
    public DiscordSettings Discord { get; set; } = new();
}

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeout { get; set; } = 30;
    public int MaxRetryCount { get; set; } = 3;
}

public class SwaggerSettings
{
    public string Title { get; set; } = "Devs Who Run API";
    public string Description { get; set; } = "API for managing developers who run";
    public string Version { get; set; } = "v1";
}

public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}

public class DiscordSettings
{
    public string BotToken { get; set; } = string.Empty;
    public string ServerId { get; set; } = string.Empty;
}

public class GitHubSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string CallbackPath { get; set; } = "/api/auth/github-callback";
    public string AuthorizationEndpoint { get; set; } = "https://github.com/login/oauth/authorize";
    public string TokenEndpoint { get; set; } = "https://github.com/login/oauth/access_token";
    public string UserInformationEndpoint { get; set; } = "https://api.github.com/user";
    public string Scope { get; set; } = "user:email";
    // should changes to prod url latest this is for local
    public string RedirectUri { get; set; } = "http://localhost:5299/api/auth/github-callback";

    public Dictionary<string, string> ClaimMappings { get; set; } = new()
    {
        { ClaimTypes.NameIdentifier, "id" },
        { ClaimTypes.Name, "name" },
        { ClaimTypes.Email, "email" },
        { "urn:github:login", "login" },
        { "urn:github:url", "html_url" },
        { "urn:github:avatar", "avatar_url" }
    };
}