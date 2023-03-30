using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services
{
    public interface IEmailSender:IScopedDependency
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}