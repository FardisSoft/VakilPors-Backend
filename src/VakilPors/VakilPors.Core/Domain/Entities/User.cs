using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VakilPors.Core.Domain.Entities;

[Table(nameof(User))]
public class User 
{
    [Key]
    public int UserId { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MaxLength(256)]
    public string Password { get; set; }

    [Required]
    [MaxLength(256)]
    public string PasswordSalt { get; set; }

    [Required]
    public bool IsActive { get; set; }

    [Required]
    public bool IsLocked { get; set; }
}

