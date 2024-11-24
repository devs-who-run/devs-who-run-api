namespace devs_who_run_api;

public enum UserType
{
    Admin,
    User,
    Conf,
    Meetup
}

public class Member
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? GitHubUserName { get; set; }
    public string? StravaProfile { get; set; }
    public UserType UserType { get; set; }
    public string? GithubUserId { get; set; }
    public string? Avatar { get; set; }
    public DateTime LastLogin { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}