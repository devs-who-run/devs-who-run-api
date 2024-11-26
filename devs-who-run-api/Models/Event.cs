using System.ComponentModel.DataAnnotations;

namespace devs_who_run_api.Models;

public class Event
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string EventName { get; set; } = string.Empty;
    
    [Required]
    public bool IsOnline { get; set; }
    
    [Required]
    public string OrganizedBy { get; set; } = string.Empty;
    
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    
    // Nav Props
    public ICollection<Conference> Conferences { get; set; } = new List<Conference>();
}
