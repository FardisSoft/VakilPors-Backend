using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using VakilPors.Shared.Entities;

namespace VakilPors.Core.Domain.Entities;


public class User : IdentityUser<int>
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public string? ForgetPasswordCode { get; set; }
    public string? ActivationCode { get; set; }
    public decimal Balance { get; set; } = 0m;
    public bool IsActive
    {
        get { return PhoneNumberConfirmed; }
        set { PhoneNumberConfirmed = value; }
    }
    public string Job { get; set; }
    public string Bio { get; set; }

    public string ProfileImageUrl { get; set; }
    public virtual ICollection<Transaction> Transactions { get; set; }
    public int? LawyerId { get; set; }
    [ForeignKey(nameof(LawyerId))]
    public virtual Lawyer? Lawyer { get; set; }
    public virtual ICollection<Chat> Chats { get; set; }
    public virtual ICollection<ChatMessage> Messages { get; set; }
    public virtual Subscribed Subscribed { get; set; }

    public virtual ICollection<UserCommentLike> CommentLikes { get; set; }

    public virtual ICollection<UserThreadLike> ThreadLikes { get; set; }
    public virtual ICollection<Event> Events { get; set; }
    public virtual ICollection<Report> reports { get; set; }
    public string Telegram { get; set; }

}

