using System.ComponentModel.DataAnnotations;
using VakilPors.Shared.Entities;

namespace VakilPors.Core.Domain.Entities;

public class Visitor : IEntity
{
    [Key]
    public int Id { get; set; }
    public string UserGUID { get; set; }
    public string IPv4 { get; set; }
    public DateTime visitTime { get; set; } = DateTime.Now;
}
