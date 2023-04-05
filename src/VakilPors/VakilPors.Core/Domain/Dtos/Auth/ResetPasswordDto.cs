using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VakilPors.Core.Domain.Dtos
{
    public record ResetPasswordDto
    {
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "Your Password is limited to {2} to {1} characters", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required]
        [Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; }
    }
}
