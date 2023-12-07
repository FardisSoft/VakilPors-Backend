using System.ComponentModel.DataAnnotations;

namespace VakilPors.Core.Domain.Dtos.Event;

public class CreateEventDto
{
    [Required]
    public string Title { get; set; }
    public string Description { get; set; }
    [Required]
    public DateTime StartTime { get; set; }
    [Required]
    public DateTime EndTime { get; set; }
    [Required]
    public int LawyerId { get; set; }
}