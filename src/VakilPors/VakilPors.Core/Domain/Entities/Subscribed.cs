using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using VakilPors.Shared.Entities;


namespace VakilPors.Core.Domain.Entities
{
    public class Subscribed : IEntity
    {
        [Key]
        public int ID { get; set; }
        public int PremiumID { get; set; }
        [ForeignKey(nameof(PremiumID))]
        public virtual Premium Premium { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public DateTime ExpireDate { get; set; } = DateTime.Now.AddYears(100);
        public int RemainingDays { get { return (DateTime.Now - ExpireDate).Days; } }
        public bool IsExpired { get { return DateTime.Now > ExpireDate; } }

    }
}
