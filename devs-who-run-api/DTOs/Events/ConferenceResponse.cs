namespace devs_who_run_api.DTOs.Events;

public record ConferenceResponse(
    int Id,
    string ConferenceName,
    int Year,
    string? Logo,
    string Website,
    string MemberName,
    string? MemberEmail,
    DateTime CreatedOn,
    DateTime UpdatedOn
    );
