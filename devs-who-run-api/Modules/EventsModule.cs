using Carter;
using devs_who_run_api.Core.Mappers;
using devs_who_run_api.DTOs.Events;
using devs_who_run_api.Models;
using Microsoft.EntityFrameworkCore;

namespace devs_who_run_api.Modules;

public class EventsModule : CarterModule
{
    public EventsModule() : base("/api")
    {
        WithTags("Events");
        IncludeInOpenApi();
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/event", async (CreateEventRequest request, DevsWhoRunDbContext db) =>
        {
            if (request is null)
                return Results.BadRequest("Event data is required");

            try
            {
                var eventEntity = new Event
                {
                    EventName = request.EventName,
                    IsOnline = request.IsOnline,
                    OrganizedBy = request.OrganizedBy,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                };

                db.Events.Add(eventEntity);
                await db.SaveChangesAsync();

                return Results.Created($"/event/{eventEntity.Id}", eventEntity);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        });

        app.MapPost("/event/{eventId}/conference", async (int eventId, CreateConferenceRequest request, DevsWhoRunDbContext db) =>
        {
            if (request is null)
                return Results.BadRequest("Conference data is required");

            var eventExists = await db.Events.AnyAsync(e => e.Id == eventId);
            if (!eventExists)
                return Results.NotFound("Event not found");

            var member = await db.Members.FindAsync(request.MemberId);
            if (member is null)
                return Results.NotFound("Member not found");

            try
            {
                var conference = new Conference
                {
                    ConferenceName = request.ConferenceName,
                    Year = request.Year,
                    Logo = request.Logo,
                    Website = request.Website,
                    MemberId = request.MemberId,
                    EventId = eventId,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                };

                db.Conferences.Add(conference);
                await db.SaveChangesAsync();
                
                var conferenceDto = ConferenceDtoMapper.Map(conference);

                return Results.Created($"/event/{eventId}/conference/{conference.Id}", conferenceDto);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        });

        app.MapGet("/event/{id}", async (int id, DevsWhoRunDbContext db) =>
        {
            var eventData = await db.Events
                .Include(e => e.Conferences)
                .ThenInclude(c => c.Member)
                .SingleOrDefaultAsync(e => e.Id == id);

            if (eventData is null)
                return Results.NotFound();
            
            var eventResponse = EventsDtoMapper.Map(eventData);

            return Results.Ok(eventResponse);
        });

        app.MapGet("/events", async (DevsWhoRunDbContext db) =>
        {
            var events = await db.Events
                .Include(e => e.Conferences)
                .ThenInclude(c => c.Member)
                .ToListAsync();
            
            var eventsDto = events.Select(EventsDtoMapper.Map).ToList();

            return Results.Ok(eventsDto);
        });
    }
}
