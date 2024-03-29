using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VakilPors.Shared.Entities;

namespace VakilPors.Core.Domain.Entities;

public class Report :IEntity
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Description { get; set; }
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }
    public int CommentId{ get; set; }
    [ForeignKey(nameof(CommentId))]
public virtual ThreadComment ThreadComment { get; set; }
    public Status Status { get; set; } = Status.PENDING;
}
