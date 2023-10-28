using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Core.Domain.Dtos.User;

namespace VakilPors.Core.Domain.Dtos.Rate
{
    public record RateUserDto
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public double RateNum { get; set; }
        public UserDto User { get; set; } 
    }
}
