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
        public DateTime ExpireDate { get; set; } = DateTime.MaxValue;
        public int RemainingDays { get { return (ExpireDate - DateTime.Now).Days; } }
        public bool IsExpired { get { return DateTime.Now > ExpireDate; } set { } }
        public string PremiumName { get { return PRName(); } }
        public int UserId { get; set; }

        private string PRName()
        {
            if (this.RemainingDays > 100)
            {
                return "Free";
            }
            else if (this.RemainingDays > 60 && this.RemainingDays < 90)
            {
                return "Gold";
            }
            else if (this.RemainingDays > 30 && this.RemainingDays < 60)
            {
                return "Silver";
            }
            else if (this.RemainingDays > 0 && this.RemainingDays < 30)
            {
                return "Bronze";
            }
            else
                return "not found";
            
        }
    }
}
