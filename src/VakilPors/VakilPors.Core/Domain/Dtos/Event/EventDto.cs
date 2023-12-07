using System.ComponentModel.DataAnnotations;

namespace VakilPors.Core.Domain.Dtos.Event;

public class EventDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int UserId { get; set; }
    public int LawyerId { get; set; }
}