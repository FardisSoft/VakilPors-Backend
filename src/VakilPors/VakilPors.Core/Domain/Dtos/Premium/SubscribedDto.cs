using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Core.Domain.Dtos.User;



namespace VakilPors.Core.Domain.Dtos.Premium
{
    public record SubscribedDto
    { 
        public int ID { get; set; }
        public virtual PremiumDto Premium { get; set; }
        public virtual UserDto User { get; set; }
        public DateTime ExpireDate { get; set; }
        public int RemainingDays { get { return (DateTime.Now - ExpireDate).Days; } }
        public bool IsExpired { get { return DateTime.Now > ExpireDate; } }

    }
}
