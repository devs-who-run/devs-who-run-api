using devs_who_run_api;

var builder = WebApplication.CreateBuilder(args);

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

var partnerConference = new[] { "TIL Conf"}
;


app.MapGet("/getPartnerConference", () => partnerConference).WithName("GetPartnerConferences").WithOpenApi();

app.MapPost("/addMember", async (DevsWhoRunDbContext db,  Member member) =>
{
    db.Add(member);
    await db.SaveChangesAsync();
    return Results.Created();
});

app.MapGet("/member/{id:int}",
    async (DevsWhoRunDbContext db, int id) =>
        await db.Members.FindAsync(id) 
            is Member member ? Results.Ok(member) : Results.NotFound());
    


// app.MapPost("/addMember", async (Member member)=> )

app.Run();
