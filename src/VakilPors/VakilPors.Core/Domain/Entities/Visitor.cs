using System.ComponentModel.DataAnnotations;
using VakilPors.Shared.Entities;

namespace VakilPors.Core.Domain.Entities;

public class Visitor : IEntity
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string UserGUID { get; set; }
    [Required]
    public string IPv4 { get; set; }
    public DateTime VisitTime { get; set; } = DateTime.Now;
}
