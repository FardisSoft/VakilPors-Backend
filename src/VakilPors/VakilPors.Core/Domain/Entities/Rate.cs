using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Shared.Entities;

namespace VakilPors.Core.Domain.Entities
{
    public class Rate :IEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Comment { get; set; }
        [Required]
        public double RateNum { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public int LawyerId { get; set; }
        [ForeignKey(nameof(LawyerId))]
        public Lawyer Lawyer { get; set; }

    }
}
