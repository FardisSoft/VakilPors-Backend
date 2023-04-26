using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Shared.Entities;
using VakilPors.Core.Domain.Entities;

namespace VakilPors.Core.Domain.Dtos.Premium
{
    public record PremiumDto
    {
       public int Id { get; set; }
       public Plan ServiceType { get; set; }
    }
}
