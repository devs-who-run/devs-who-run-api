using System.Net.Http.Headers;
using System.Text.Json;
using DevsWhoRun.Api.Configurations;
using DevsWhoRun.Api.Modules.Discord.DTOs;
using DevsWhoRun.Api.Modules.Discord.Services.Interfaces;

namespace DevsWhoRun.Api.Modules.Discord.Services;

public class DiscordService(HttpClient httpClient, AppSettings appSettings) : IDiscordService
{
    private const string BaseUrl = "https://discord.com/api/v10";
    public async Task<DiscordMembers> GetMembersCountAsync()
    {
        try
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bot", appSettings.Discord.BotToken);
            var response = await httpClient.GetAsync($"{BaseUrl}/guilds/{appSettings.Discord.ServerId}?with_counts=true");
            response.EnsureSuccessStatusCode();
            
            var jsonString = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(jsonString);
            
            var totalMembers = document.RootElement.GetProperty("approximate_member_count").GetInt32();
            var onlineMembers = document.RootElement.GetProperty("approximate_presence_count").GetInt32();
            return new DiscordMembers(totalMembers, onlineMembers);
        }
        catch (Exception e)
        {
            // Console.WriteLine(e);
            // throw;
            // added a fallback to return 0 members if the request fails will be removed in future as end to end setup
            return new DiscordMembers(0, 0);
        }
    }
}