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
        public string New_Password { get; set; }
    }
}
