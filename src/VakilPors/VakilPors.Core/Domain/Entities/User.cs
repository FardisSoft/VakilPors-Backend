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
    public decimal Balance { get; set; }=0m;
    public bool IsActive
    {
        get { return PhoneNumberConfirmed; }
        set { PhoneNumberConfirmed = value; }
    }
    public ICollection<Tranaction> Tranactions { get; set; }
    public int? LawyerId { get; set; }
    [ForeignKey(nameof(LawyerId))]
    public Lawyer Lawyer { get; set; }
}

