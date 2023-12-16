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
    public int ThreadId { get; set; }
    [ForeignKey(nameof(ThreadId))]
    public virtual ForumThread Thread { get; set; }
    //TODO: create enum(install extension)
}
