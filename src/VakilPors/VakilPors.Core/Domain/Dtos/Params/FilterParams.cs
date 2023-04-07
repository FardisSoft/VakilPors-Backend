using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VakilPors.Core.Domain.Dtos.Params
{
    public record FilterParams
    {
        public string Q { get; set; }
        public string Sort { get; set; }
        public bool IsAscending { get; set; }
    }
}