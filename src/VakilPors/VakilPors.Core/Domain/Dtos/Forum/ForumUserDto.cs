using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VakilPors.Core.Domain.Dtos
{
    public class ForumUserDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public bool IsLawyer { get; set; }
        public bool IsPremium { get; set; }
    }
}
