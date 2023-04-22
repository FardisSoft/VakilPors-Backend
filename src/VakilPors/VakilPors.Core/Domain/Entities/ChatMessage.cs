using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VakilPors.Shared.Entities;

namespace VakilPors.Core.Domain.Entities
{
    public class ChatMessage : IEntity
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public bool IsFile { get; set; } = false;
        public bool IsRead { get; set; } = false;
        public int SenderId { get; set; }
        public int ChatId { get; set; }
        [ForeignKey(nameof(SenderId))]
        public User Sender { get; set; }
        [ForeignKey(nameof(ChatId))]
        public Chat Chat { get; set; }
    }
}