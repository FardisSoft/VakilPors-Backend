using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VakilPors.Core.Domain.Dtos.Rate
{
    public record RateDto
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public double RateNum { get; set; }

    }
}
