namespace devs_who_run_api.Configurations;

public class AppSettings
{
    public DatabaseSettings Database { get; set; } = new();
    public SwaggerSettings Swagger { get; set; } = new();
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