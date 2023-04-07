using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VakilPors.Core.Domain.Entities
{
    public class Lawyer
    {
        [Key]
        public int Id { get; set; }
        public double Rating { get; set; }
        public string ParvandeNo { get; set; }
        public bool IsAuthorized { get; set; }=false;
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}