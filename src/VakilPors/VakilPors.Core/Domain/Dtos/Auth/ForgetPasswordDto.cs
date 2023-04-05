using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VakilPors.Core.Domain.Dtos
{
    public record ForgetPasswordDto
    {
        [Required]
        public string PhoneNumber { get; set; }
    }
}
