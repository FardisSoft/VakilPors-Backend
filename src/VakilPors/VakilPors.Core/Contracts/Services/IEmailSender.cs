using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services
{
    public interface IEmailSender : IScopedDependency
    {
        Task SendEmailAsync(string email, string name, string subject, string message,bool useHtml = true);
    }
}