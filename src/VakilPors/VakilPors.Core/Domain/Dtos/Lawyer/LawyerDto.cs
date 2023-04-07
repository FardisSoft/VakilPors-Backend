using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VakilPors.Core.Domain.Dtos.User;

namespace VakilPors.Core.Domain.Dtos.Lawyer
{
    public record LawyerDto
    {
        public double Rating { get; set; }
        public string ParvandeNo { get; set; }
        public bool IsAuthorized { get; set; }=false;
        public UserDto User { get; set; }
    }
}