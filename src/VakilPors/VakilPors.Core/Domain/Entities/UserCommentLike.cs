using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Shared.Entities;

namespace VakilPors.Core.Domain.Entities
{
    public class UserCommentLike : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Required]
        public int CommentId { get; set; }

        [ForeignKey(nameof(CommentId))]
        public ThreadComment Comment { get; set; }
    }
}
