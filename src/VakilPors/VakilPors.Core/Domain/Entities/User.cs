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
    public virtual ICollection<Tranaction> Tranactions { get; set; }
    public int? LawyerId { get; set; }
    [ForeignKey(nameof(LawyerId))]
    public virtual Lawyer? Lawyer { get; set; }
    public virtual ICollection<Chat> Chats { get; set; }
    public virtual ICollection<ChatMessage> Messages { get; set; }
}

