namespace devs_who_run_api;

/// <summary>
/// Defines the type of user and their associated permissions in the system
/// </summary>
public enum UserType
{
    /// <summary>
    /// Default user type with basic permissions
    /// </summary>
    User = 0,
    /// <summary>
    /// Administrator with full system access
    /// </summary>
    Admin,
    /// <summary>
    /// Conference organizer with event management permissions
    /// </summary>
    Conference,
    /// <summary>
    /// Meetup organizer with group management permissions
    /// </summary>
    MeetupOrganizer
}

public class Member
{
    public int Id { get; set; }
    public string? Name { get; set; }
    [EmailAddress]
    [MaxLength(256)]
    public string? Email { get; set; }

    [MaxLength(100)]
    public string? GitHubUserName { get; set; }

    [MaxLength(100)]
    [Url]
    public string? StravaProfile { get; set; }
    public UserType UserType { get; set; }
    public string? GithubUserId { get; set; }
    public string? Avatar { get; set; }
    public DateTime LastLogin { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}