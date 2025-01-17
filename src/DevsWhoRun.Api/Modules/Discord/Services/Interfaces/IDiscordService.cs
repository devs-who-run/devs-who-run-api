using DevsWhoRun.Api.Modules.Discord.DTOs;

namespace DevsWhoRun.Api.Modules.Discord.Services.Interfaces;

public interface IDiscordService
{
    Task<DiscordMembers> GetMembersCountAsync();
}