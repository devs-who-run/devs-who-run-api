using devs_who_run_api;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DevsWhoRunDbContext>(options =>
    // options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    options.UseNpgsql("Server=localhost;Port=5432;Database=devswhorun;User Id=postgres;Password=admin;", o => o.MapEnum<UserType>("usertype"))
);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var partnerConference = new[] { "TIL Conf" }
;


app.MapGet("/getPartnerConference", () => partnerConference).WithName("GetPartnerConferences").WithOpenApi();

app.MapPost("/addMember", async (Member member, DevsWhoRunDbContext db) =>
{
    if (member == null)
        return Results.BadRequest("Member data is required");

    if (string.IsNullOrEmpty(member.Email))
        return Results.BadRequest("Email is required");

    try
    {
        member.CreatedOn = DateTime.UtcNow;
        member.UpdatedOn = DateTime.UtcNow;

        db.Members.Add(member);
        await db.SaveChangesAsync();

        return Results.Created($"/member/{member.Id}", member);
    }
    catch (Exception ex)
    {
        return Results.Problem("Failed to add member", statusCode: 500);
    }
});

app.MapGet("/member/{id:int}",
    async (int id, DevsWhoRunDbContext db) =>
        await db.Members.FindAsync(id)
            is Member member ? Results.Ok(member) : Results.NotFound());


app.Run();
