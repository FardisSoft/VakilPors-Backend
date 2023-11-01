using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Core.Domain.Dtos.User;

namespace VakilPors.Core.Domain.Dtos.Premium
{
    public record GetAllSubscriptionDto
    {
        public int Id { get; set; }
        public UserDto User { get; set; }
        public PremiumDto Premium { get; set; }

    }
}
