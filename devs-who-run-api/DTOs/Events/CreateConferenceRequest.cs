using System.ComponentModel.DataAnnotations;

namespace devs_who_run_api.DTOs.Events;

public class CreateConferenceRequest
{
    [Required(ErrorMessage = "Conference name is required")]
    public string ConferenceName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Year is required")]
    [Range(2000, 2100, ErrorMessage = "Year must be between 2000 and 2100")]
    public int Year { get; set; }
    
    public string? Logo { get; set; }
    
    [Required(ErrorMessage = "Website URL is required")]
    [Url(ErrorMessage = "Please provide a valid URL")]
    public string Website { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Member ID is required")]
    public int MemberId { get; set; }
}
