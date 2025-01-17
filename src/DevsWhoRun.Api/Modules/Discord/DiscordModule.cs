using Carter;
using DevsWhoRun.Api.Modules.Discord.Services.Interfaces;

namespace DevsWhoRun.Api.Modules.Discord;

public class DiscordModule : CarterModule
{
    public DiscordModule() : base("/api")
    {
        WithTags("Discord");
        IncludeInOpenApi();
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/discord/members", async (IDiscordService discordService) =>
        {
            try
            {
                var counts = await discordService.GetMembersCountAsync();
                return Results.Ok(counts);
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message, statusCode: 500);
            }
        });
    }
}