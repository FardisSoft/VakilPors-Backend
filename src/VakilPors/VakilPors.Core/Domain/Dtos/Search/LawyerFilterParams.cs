using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VakilPors.Core.Domain.Dtos.Search
{
    public record LawyerFilterParams
    {
        public double Rating { get; set; } = 0d;
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string MemberOf { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;





    }
}
