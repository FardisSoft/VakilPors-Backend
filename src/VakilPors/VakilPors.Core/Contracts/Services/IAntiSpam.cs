using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VakilPors.Core.Contracts.Services
{
    public interface IAntiSpam
    {
        Task<string> IsSpam(string Text);
    }
}
