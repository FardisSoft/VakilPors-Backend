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
    public enum Plan
    {
        Free=0,
        Bronze , 
        Siler,
        Gold,
    }
    public class Premium:IEntity
    {
        [Key]
        public int Id { get; set; }
        public Plan ServiceType { get; set; }


    }
}
