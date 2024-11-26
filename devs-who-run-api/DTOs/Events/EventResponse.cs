namespace devs_who_run_api.DTOs.Events;

public record EventResponse(
    int Id, string EventName,
    bool IsOnline,
    string OrganizedBy,
    DateTime CreatedOn,
    DateTime UpdatedOn,
    ICollection<ConferenceResponse> Conferences
    );
