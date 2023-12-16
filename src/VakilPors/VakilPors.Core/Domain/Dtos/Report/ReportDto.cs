using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Core.Domain.Dtos.User;

namespace VakilPors.Core.Domain.Dtos.Report
{
    public record ReportDto
    {
    public int Id{get;set;}
    public string Description { get; set; }
    public UserDto User { get; set; }
    public ThreadDto Thread { get; set; }

        
    }
}
