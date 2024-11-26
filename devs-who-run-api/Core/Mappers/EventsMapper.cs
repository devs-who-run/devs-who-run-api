using devs_who_run_api.DTOs.Events;
using devs_who_run_api.Models;

namespace devs_who_run_api.Core.Mappers;

public static class EventsDtoMapper
{
    public static EventResponse Map(Event eventData)
    {
        return new EventResponse(
            eventData.Id,
            eventData.EventName,
            eventData.IsOnline,
            eventData.OrganizedBy,
            eventData.CreatedOn,
            eventData.UpdatedOn,
            Conferences: eventData.Conferences
                .Select(ConferenceDtoMapper.Map)
                .ToList()
        );
    }
}
