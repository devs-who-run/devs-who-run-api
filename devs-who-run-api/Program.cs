using devs_who_run_api;
using devs_who_run_api.Configurations;
using Microsoft.EntityFrameworkCore;
using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using devs_who_run_api.Modules.Auth.Services;
using devs_who_run_api.Modules.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Configure app settings
var appSettings = new AppSettings();
builder.Configuration.Bind(appSettings);
builder.Services.AddSingleton(appSettings);

// Configure authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = appSettings.Jwt.Issuer,
        ValidAudience = appSettings.Jwt.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Jwt.Key))
    };
}).AddOAuth("GitHub", options =>
{
    options.ClientId = appSettings.GitHub.ClientId;
    options.ClientSecret = appSettings.GitHub.ClientSecret;
    options.CallbackPath = appSettings.GitHub.CallbackPath;
    
    options.AuthorizationEndpoint = appSettings.GitHub.AuthorizationEndpoint;
    options.TokenEndpoint = appSettings.GitHub.TokenEndpoint;
    options.UserInformationEndpoint = appSettings.GitHub.UserInformationEndpoint;
    
    // Configure claim mappings from settings
    foreach (var mapping in appSettings.GitHub.ClaimMappings)
    {
        options.ClaimActions.MapJsonKey(mapping.Key, mapping.Value);
    }
});

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

// Register services
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddCarter();
builder.Services.AddAuthorization();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        //TODO: Will need to change this to allow only the client origin
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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
    
    // Configure JWT authentication in Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
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
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapCarter();

app.Run();
