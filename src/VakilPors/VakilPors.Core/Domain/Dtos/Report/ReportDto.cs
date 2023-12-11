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
    public string Description { get; set; }
    public UserDto User { get; set; }
    public virtual ThreadCommentDto ThreadComment { get; set; }

        
    }
}
