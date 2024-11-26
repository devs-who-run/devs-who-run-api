using Carter;
using Microsoft.EntityFrameworkCore;

namespace devs_who_run_api.Modules;

public class MembersModule : CarterModule
{
    public MembersModule() : base("/api")
    {
        WithTags("Members");
        IncludeInOpenApi();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/partnerMeetup", async (DevsWhoRunDbContext db) =>
        {
            try
            {
                var meetups = await db.Members.Where(m => m.UserType == UserType.Meetup).ToListAsync();

                return Results.Ok(meetups);
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message, statusCode: 500);
            }
        });

        app.MapGet("/partnerConference", async (DevsWhoRunDbContext db) =>
        {
            try
            {
                var conferences = await db.Members.Where(m => m.UserType == UserType.Conf).ToListAsync();

                return Results.Ok(conferences);
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message, statusCode: 500);
            }
        });

        app.MapPost("/member", async (Member member, DevsWhoRunDbContext db) =>
        {
            if (member is null)
                return Results.BadRequest("Member data is required");

            if (string.IsNullOrEmpty(member.Email))
                return Results.BadRequest("Email is required");
            
            var isEmailExist = await db.Members.AnyAsync(m => m.Email == member.Email);
            if(isEmailExist) 
                return Results.BadRequest("Email already exists");

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
                return Results.Problem(ex.Message, statusCode: 500);
            }
        });

        app.MapGet("/member/{id}", async (int id, DevsWhoRunDbContext db) =>
        {
            var member = await db.Members.SingleOrDefaultAsync(m => m.Id == id);
            return member is not null ? Results.Ok(member) : Results.NotFound();
        });
        
        app.MapGet("/memberByEmail/{email}", async (string email, DevsWhoRunDbContext db) =>
        {
            var member = await db.Members.SingleOrDefaultAsync(m => m.Email == email);
            return member is not null ? Results.Ok(member) : Results.NotFound();
        });
    }
}