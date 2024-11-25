using devs_who_run_api;
using devs_who_run_api.Configurations;
using Microsoft.EntityFrameworkCore;

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

app.MapGet("/getPartnerConference", MembersEndpoint.GetPartnerConferences);

app.MapGet("/getPartnerMeetup", MembersEndpoint.GetPartnerMeetup);

app.MapPost("/addMember", MembersEndpoint.AddMembers);

app.MapGet("/getMemberByEmail/{email}", async (string email, DevsWhoRunDbContext db) =>
    await db.Members.FirstOrDefaultAsync(m => m.Email == email)
        is Member member
        ? Results.Ok((object?)member)
        : Results.NotFound());

app.MapGet("/member/{id:int}",
    async (int id, DevsWhoRunDbContext db) => await db.Members.FindAsync(id)
        is Member member
        ? Results.Ok((object?)member)
        : Results.NotFound());

app.Run();
