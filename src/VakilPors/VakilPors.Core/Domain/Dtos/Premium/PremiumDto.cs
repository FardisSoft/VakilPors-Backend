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
    public record PremiumDto
    {
        public int Id { get; set; }
        //public int UserId { get; set; }
        public UserDto User { get; set; }
        [RegularExpression("^(gold|silver|bronze)$")]
        public string ServiceType { get; set; }
        public DateTime ExpireDate { get; set; }
        public int RemainingDays { get; set; }
        public bool IsExpired { get; set; }
    }
}
