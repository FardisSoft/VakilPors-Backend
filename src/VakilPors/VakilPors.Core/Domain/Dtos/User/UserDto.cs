using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VakilPors.Core.Domain.Dtos.Payment;

namespace VakilPors.Core.Domain.Dtos.User
{
    public record UserDto
    {
        public string Name { get; set; }
        public virtual string UserName { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string Email { get; set; }
        public decimal Balance { get; set; } = 0m;
        public virtual bool PhoneNumberConfirmed { get; set; }

        public bool IsActive
        {
            get { return PhoneNumberConfirmed; }
            set { PhoneNumberConfirmed = value; }
        }
        public ICollection<TranactionDto> Tranactions { get; set; }
    }
}