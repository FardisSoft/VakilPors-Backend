using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VakilPors.Data
{
    public class VakilPorsDbContext : DbContext
    {
        public VakilPorsDbContext(DbContextOptions<VakilPorsDbContext> options) : base(options)
        {
        }

    }
}
