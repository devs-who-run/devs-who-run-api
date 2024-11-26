using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace devs_who_run_api.Models;

public class Conference
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string ConferenceName { get; set; } = string.Empty;
    
    [Required]
    public int Year { get; set; }
    
    public string? Logo { get; set; }
    
    [Required]
    public string Website { get; set; } = string.Empty;
    
    [Required]
    public int MemberId { get; set; }
    
    [Required]
    public int EventId { get; set; }
    
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    
    // Nav props
    [ForeignKey("MemberId")]
    public Member Member { get; set; } = null!;
    
    [ForeignKey("EventId")]
    public Event Event { get; set; } = null!;
}
