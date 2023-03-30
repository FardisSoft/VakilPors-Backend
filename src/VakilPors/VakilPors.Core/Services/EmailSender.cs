using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VakilPors.Core.Contracts.Services;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using Microsoft.Extensions.Configuration;
using MimeKit.Text;

namespace VakilPors.Core.Services
{
    public class EmailSender : IEmailSender
    {
        private string _fromName;
        private string _fromEmail;
        private string _host;
        private int _port;
        private string _userName;
        private string _password;
        private bool _useSSL;
        
        public EmailSender(IConfiguration configuration)
        {
            _fromName=Environment.GetEnvironmentVariable("EMAIL_NAME",EnvironmentVariableTarget.User) ?? configuration["Email:Name"];
            _fromEmail=Environment.GetEnvironmentVariable("EMAIL_FROM",EnvironmentVariableTarget.User) ?? configuration["Email:From"];
            _host=Environment.GetEnvironmentVariable("EMAIL_HOST",EnvironmentVariableTarget.User) ?? configuration["Email:Host"];
            _port=Convert.ToInt32(Environment.GetEnvironmentVariable("EMAIL_PORT",EnvironmentVariableTarget.User) ?? configuration["Email:Port"]);
            _userName=Environment.GetEnvironmentVariable("EMAIL_USERNAME",EnvironmentVariableTarget.User) ?? configuration["Email:Username"];
            _password=Environment.GetEnvironmentVariable("EMAIL_PASSWORD",EnvironmentVariableTarget.User) ?? configuration["Email:Password"];
            _useSSL=Convert.ToBoolean(Environment.GetEnvironmentVariable("EMAIL_USESSL",EnvironmentVariableTarget.User) ?? configuration["Email:UseSSL"]);
        }

        public async Task SendEmailAsync(string email, string name, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_fromName, _fromEmail));
            message.To.Add(new MailboxAddress(name, email));
            message.Subject = subject;

            message.Body = new TextPart(TextFormat.Html)
            {
                Text = htmlMessage
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_host, _port, _useSSL);

                await client.AuthenticateAsync(_userName, _password);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}