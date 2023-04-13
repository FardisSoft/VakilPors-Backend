

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using VakilPors.Shared.Entities;

namespace VakilPors.Core.Domain.Entities;

public class ThreadComment : IEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Text { get; set; }

    public int LikeCount { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }

    public int ThreadId { get; set; }

    [ForeignKey(nameof(ThreadId))]
    public Thread Thread { get; set; }
}

