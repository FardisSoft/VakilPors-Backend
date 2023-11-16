using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services
{
    public interface ISMSSender : ISingletonDependency
    {
        Task<int> SendSmsAsync(string number, string message);
    }
}