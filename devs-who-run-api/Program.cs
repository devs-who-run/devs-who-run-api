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
