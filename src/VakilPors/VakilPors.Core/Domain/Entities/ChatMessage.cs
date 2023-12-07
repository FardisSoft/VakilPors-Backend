using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VakilPors.Shared.Entities;

namespace VakilPors.Core.Domain.Entities;

public class ChatMessage : IEntity
{
    [Key]
    public int Id { get; set; }
    public string Message { get; set; }
    public bool IsFile { get; set; } = false;
    public string ContentType { get; set; } = "text/plain";
    public bool IsCall { get; set; } = false;
    public CallStatus CallStatus { get; set; } = CallStatus.WAITING;
    public bool IsDeleted { get; set; } = false;
    public bool IsEdited { get; set; } = false;
    public bool IsRead { get; set; } = false;
    public DateTime SendTime { get; set; } = DateTime.Now;
    public int SenderId { get; set; }
    public int ChatId { get; set; }
    public int? ReplyId { get; set; }
    [ForeignKey(nameof(ReplyId))]
    public ChatMessage? ReplyMessage { get; set; }
    [ForeignKey(nameof(SenderId))]
    public virtual User Sender { get; set; }
    [ForeignKey(nameof(ChatId))]
    public virtual Chat Chat { get; set; }
}
public enum CallStatus
{
    WAITING = 0,
    ACCEPT,
    REJECT
}