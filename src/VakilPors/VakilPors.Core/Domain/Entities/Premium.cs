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
    public class Premium:IEntity
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public string ServiceType { get; set; }
        public DateTime ExpireDate { get; set; }
        public int RemainingDays { get; set; }
        public bool IsExpired { get; set; }

    }
}
