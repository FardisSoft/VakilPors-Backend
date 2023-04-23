using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VakilPors.Core.Domain.Entities
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }
        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
    }
}