using devs_who_run_api.DTOs.Events;
using devs_who_run_api.Models;

namespace devs_who_run_api.Core.Mappers;

public static class ConferenceDtoMapper
{
    public static ConferenceResponse Map(Conference conference)
    {
        return new ConferenceResponse(
            conference.Id,
            conference.ConferenceName,
            conference.Year,
            conference.Logo,
            conference.Website,
            MemberName: $"{conference.Member.FirstName} {conference.Member.LastName}",
            conference.Member.Email,
            conference.CreatedOn,
            conference.UpdatedOn
        );
    }
}