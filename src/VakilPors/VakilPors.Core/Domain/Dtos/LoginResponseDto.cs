using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VakilPors.Core.Domain.Dtos
{
    public record LoginResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}