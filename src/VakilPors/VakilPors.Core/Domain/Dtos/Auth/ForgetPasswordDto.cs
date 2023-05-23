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
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool useSms { get; set; } = true;
    }
}
