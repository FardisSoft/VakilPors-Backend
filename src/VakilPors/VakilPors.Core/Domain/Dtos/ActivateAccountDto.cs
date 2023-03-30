using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VakilPors.Core.Domain.Dtos
{
    public class ActivateAccountDto
    {
        public string PhoneNumber { get; set; }
        
        public string Code { get; set; }
        
    }
}
