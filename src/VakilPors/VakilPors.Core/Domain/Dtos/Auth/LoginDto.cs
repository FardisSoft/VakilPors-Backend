using System.ComponentModel.DataAnnotations;

namespace VakilPors.Core.Domain.Dtos;

public record LoginDto
{
    [Required]
    [Phone(ErrorMessage = "Your Phone Number is not valid")]
    public string PhoneNumber { get; set; }

    [Required]
    [StringLength(30, ErrorMessage = "Your Password is limited to {2} to {1} characters", MinimumLength = 6)]
    public string Password { get; set; }
}

