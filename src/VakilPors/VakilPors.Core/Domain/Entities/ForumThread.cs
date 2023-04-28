
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VakilPors.Shared.Entities;

namespace VakilPors.Core.Domain.Entities;

public class ForumThread : IEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string Description { get; set; }

    public int LikeCount { get; set; }

    public int UserId { get; set; }

    public DateTime CreateDate { get; set; }

    public bool HasAnswer { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }

    public virtual ICollection<UserThreadLike> UserLikes { get; set; }
}

