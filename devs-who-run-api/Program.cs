using devs_who_run_api;
using devs_who_run_api.Configurations;
using Microsoft.EntityFrameworkCore;
using Carter;

var builder = WebApplication.CreateBuilder(args);

// Configure app settings
var appSettings = new AppSettings();
builder.Configuration.Bind(appSettings);
builder.Services.AddSingleton(appSettings);

// Configure database
builder.Services.AddDbContext<DevsWhoRunDbContext>(options =>
{
    options.UseNpgsql(
        appSettings.Database.ConnectionString,
        npgsqlOptions =>
        {
            npgsqlOptions.MapEnum<UserType>("usertype");
            npgsqlOptions.CommandTimeout(appSettings.Database.CommandTimeout);
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: appSettings.Database.MaxRetryCount,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null);
        });
});

builder.Services.AddCarter();

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(appSettings.Swagger.Version, new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = appSettings.Swagger.Title,
        Description = appSettings.Swagger.Description,
        Version = appSettings.Swagger.Version
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapCarter();

app.Run();
