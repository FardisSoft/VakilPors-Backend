using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VakilPors.Core.Domain.Dtos
{
    public record SignUpDto: LoginDto
    {
        [Required]
        public string Name { get; set; }
    }
}