using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VakilPors.Core.Domain.Entities
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public bool IsDeleted { get; set; } = false;
        [ForeignKey(nameof(SenderId))]
        public User Sender { get; set; }
        [ForeignKey(nameof(ReceiverId))]
        public User Receiver { get; set; }
        public ICollection<ChatMessage> ChatMessages { get; set; }
    }
}