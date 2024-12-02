using System.ComponentModel.DataAnnotations;

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
    Conf,
    /// <summary>
    /// Meetup organizer with group management permissions
    /// </summary>
    Meetup
}

public class Member
{
    public int Id { get; set; }
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    [EmailAddress]
    [MaxLength(256)]
    [Required]
    public string? Email { get; set; }

    [MaxLength(100)]
    [Required]
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
    
    // Authentication fields
    public string? PasswordHash { get; set; }
    public bool IsEmailVerified { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}