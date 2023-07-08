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
using Microsoft.Extensions.Logging;

namespace VakilPors.Core.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private string _fromName;
        private string _fromEmail;
        private string _host;
        private int _port;
        private string _userName;
        private string _password;
        private bool _useSSL;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _fromName = Environment.GetEnvironmentVariable("EMAIL_NAME") ?? Environment.GetEnvironmentVariable("EMAIL_NAME", EnvironmentVariableTarget.User) ?? configuration["Email:Name"];
            _fromEmail = Environment.GetEnvironmentVariable("EMAIL_FROM") ?? Environment.GetEnvironmentVariable("EMAIL_FROM", EnvironmentVariableTarget.User) ?? configuration["Email:From"];
            _host = Environment.GetEnvironmentVariable("EMAIL_HOST") ?? Environment.GetEnvironmentVariable("EMAIL_HOST", EnvironmentVariableTarget.User) ?? configuration["Email:Host"];
            _port = Convert.ToInt32(Environment.GetEnvironmentVariable("EMAIL_PORT") ?? Environment.GetEnvironmentVariable("EMAIL_PORT", EnvironmentVariableTarget.User) ?? configuration["Email:Port"]);
            _userName = Environment.GetEnvironmentVariable("EMAIL_USERNAME") ?? Environment.GetEnvironmentVariable("EMAIL_USERNAME", EnvironmentVariableTarget.User) ?? configuration["Email:Username"];
            _password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? Environment.GetEnvironmentVariable("EMAIL_PASSWORD", EnvironmentVariableTarget.User) ?? configuration["Email:Password"];
            _useSSL = Convert.ToBoolean(Environment.GetEnvironmentVariable("EMAIL_USESSL") ?? Environment.GetEnvironmentVariable("EMAIL_USESSL", EnvironmentVariableTarget.User) ?? configuration["Email:UseSSL"]);
            this._logger = logger;
        }

        public async Task SendEmailAsync(string email, string name, string subject, string htmlMessage, bool useHtml = true)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_fromName, _fromEmail));
                message.To.Add(new MailboxAddress(name, email));
                message.Subject = subject;
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = (useHtml ? html.Replace("{{content}}", htmlMessage).Replace("{{title}}", subject) : htmlMessage)
                };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_host, _port, _useSSL);

                    await client.AuthenticateAsync(_userName, _password);

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (System.Exception)
            {
                _logger.LogError($"An Error occured while sending Email {_fromEmail} to {email} with subj:{subject} name:{name}");
            }
        }
        private string html = "<!doctypehtml><html dir=rtl><meta charset=utf-8><meta content=\"ie=edge\"http-equiv=x-ua-compatible><title>Email Confirmation</title><meta content=\"width=device-width,initial-scale=1\"name=viewport><style>@media screen{@font-face{font-family:'Source Sans Pro';font-style:normal;font-weight:400;src:local('Source Sans Pro Regular'),local('SourceSansPro-Regular'),url(https://fonts.gstatic.com/s/sourcesanspro/v10/ODelI1aHBYDBqgeIAH2zlBM0YzuT7MdOe03otPbuUS0.woff) format('woff')}@font-face{font-family:'Source Sans Pro';font-style:normal;font-weight:700;src:local('Source Sans Pro Bold'),local('SourceSansPro-Bold'),url(https://fonts.gstatic.com/s/sourcesanspro/v10/toadOcfmlt9b38dHJxOBGFkQc6VGVFSmCnC_l7QZG60.woff) format('woff')}}a,body,table,td{-ms-text-size-adjust:100%;-webkit-text-size-adjust:100%}table,td{mso-table-rspace:0;mso-table-lspace:0}img{-ms-interpolation-mode:bicubic}a[x-apple-data-detectors]{font-family:inherit!important;font-size:inherit!important;font-weight:inherit!important;line-height:inherit!important;color:inherit!important;text-decoration:none!important}div[style*=\"margin: 16px 0;\"]{margin:0!important}body{width:100%!important;height:100%!important;padding:0!important;margin:0!important}table{border-collapse:collapse!important}a{color:#1a82e2}img{height:auto;line-height:100%;text-decoration:none;border:0;outline:0}</style><body style=background-color:#e9ecef><div class=preheader style=display:none;max-width:0;max-height:0;overflow:hidden;font-size:1px;line-height:1px;color:#fff;opacity:0>A preheader is the short summary text that follows the subject line when an email is viewed in the inbox.</div><table border=0 cellpadding=0 cellspacing=0 width=100%><tr><td align=center bgcolor=#e9ecef><!--[if (gte mso 9)|(IE)]><table border=0 cellpadding=0 cellspacing=0 width=600 align=center><tr><td align=center valign=top width=600><![endif]--><table border=0 cellpadding=0 cellspacing=0 width=100% style=max-width:600px><tr><td align=center style=\"padding:36px 24px\"valign=top><a href=https://www.blogdesire.com style=display:inline-block target=_blank><img alt=Logo border=0 src=https://fardissoft.ir/logoFS.jpg style=display:block;width:48px;max-width:48px;min-width:48px width=48></a></table><!--[if (gte mso 9)|(IE)]><![endif]--><tr><td align=center bgcolor=#e9ecef><!--[if (gte mso 9)|(IE)]><table border=0 cellpadding=0 cellspacing=0 width=600 align=center><tr><td align=center valign=top width=600><![endif]--><table border=0 cellpadding=0 cellspacing=0 width=100% style=max-width:600px><tr><td align=right bgcolor=#ffffff style=\"padding:36px 24px 0;font-family:'Source Sans Pro',Helvetica,Arial,sans-serif;border-top:3px solid #d4dadf\"><h1 style=margin:0;font-size:32px;font-weight:700;letter-spacing:-1px;line-height:48px>{{title}}</h1></table><!--[if (gte mso 9)|(IE)]><![endif]--><tr><td align=center bgcolor=#e9ecef><!--[if (gte mso 9)|(IE)]><table border=0 cellpadding=0 cellspacing=0 width=600 align=center><tr><td align=center valign=top width=600><![endif]--><table border=0 cellpadding=0 cellspacing=0 width=100% style=max-width:600px><tr><td align=right bgcolor=#ffffff style=\"padding:24px;font-family:'Source Sans Pro',Helvetica,Arial,sans-serif;font-size:16px;line-height:24px\"><p style=margin:0>{{content}}</table><!--[if (gte mso 9)|(IE)]><![endif]--><tr><td align=center bgcolor=#e9ecef style=padding:24px><!--[if (gte mso 9)|(IE)]><table border=0 cellpadding=0 cellspacing=0 width=600 align=center><tr><td align=center valign=top width=600><![endif]--><table border=0 cellpadding=0 cellspacing=0 width=100% style=max-width:600px>از طرف وب سایت <a href=\"https://fardissoft.ir\" target=\"_blank\">وکیل پرس</a></table><!--[if (gte mso 9)|(IE)]><![endif]--></table>";
    }
}