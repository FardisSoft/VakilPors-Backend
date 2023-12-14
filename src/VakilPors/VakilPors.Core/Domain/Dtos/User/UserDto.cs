using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Payment;
using VakilPors.Core.Domain.Dtos.Premium;
using VakilPors.Core.Domain.Entities;

namespace VakilPors.Core.Domain.Dtos.User
{
    public record UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual string UserName { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string Email { get; set; }
        public decimal Balance { get; set; } = 0m;
        public virtual bool PhoneNumberConfirmed { get; set; }
        public bool IsActive { get; set; }
        public string Job { get; set; }
        public string Bio { get; set; }

        public bool IsPremium { get; set; }
        public string PremiumLevel { get; set; }
        public IFormFile ProfileImage { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Telegram { get; set; }
        public string RoleName { get; set; }
        public LawyerDto Lawyer { get; set; }

    }
}