using Microsoft.EntityFrameworkCore;

namespace devs_who_run_api;

public static class MembersEndpoint
{
    public static async Task<IResult> GetPartnerMeetup(DevsWhoRunDbContext db)
    {
        try
        {
            var meetups = await db.Members.Where(m => m.UserType == UserType.Meetup).ToListAsync();

            if (!meetups.Any())
            {
                return Results.NotFound(new { Message = "No partner meetup found." });
            }

            return Results.Ok(meetups);
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message, statusCode: 500);
        }
    }

    public static async Task<IResult> GetPartnerConferences(DevsWhoRunDbContext db)
    {
        try
        {
            var conferences = await db.Members.Where(m => m.UserType == UserType.Conf).ToListAsync();

            if (!conferences.Any())
            {
                return Results.NotFound(new { Message = "No partner conferences found." });
            }

            return Results.Ok(conferences);
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message, statusCode: 500);
        }
    }

    public static async Task<IResult> AddMembers(Member member, DevsWhoRunDbContext db)
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
            return Results.Problem(ex.Message, statusCode: 500);
        }
    }

    
}