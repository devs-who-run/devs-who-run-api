namespace devs_who_run_api.DTOs.Events;

public record CreateEventRequest(string EventName, bool IsOnline, string OrganizedBy);