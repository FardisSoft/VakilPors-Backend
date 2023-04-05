using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VakilPors.Core.Domain.Dtos.Payment
{
    public record RequestPaymentDto{
        public long Amount { get; set; }
        public string Description { get; set; }
    }

}