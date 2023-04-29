using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Exceptions;

namespace VakilPors.Core.Services
{
    public class SMSSender : ISMSSender
    {
        private readonly ILogger<SMSSender> logger;
        private string _smsSenderNumber;
        private string _smsUsername;
        private string _smsPassword;

        public SMSSender(IConfiguration configuration, ILogger<SMSSender> logger)
        {
            _smsSenderNumber = Environment.GetEnvironmentVariable("RAYGAN_SMS_SENDER_NUMBER", EnvironmentVariableTarget.User) ?? configuration["RAYGAN_SMS:SENDER_NUMBER"];
            _smsUsername = Environment.GetEnvironmentVariable("RAYGAN_SMS_USERNAME", EnvironmentVariableTarget.User) ?? configuration["RAYGAN_SMS:USERNAME"];
            _smsPassword = Environment.GetEnvironmentVariable("RAYGAN_SMS_PASSWORD", EnvironmentVariableTarget.User) ?? configuration["RAYGAN_SMS:PASSWORD"];
            this.logger = logger;
            logger.LogInformation($"Sender Number is:{_smsSenderNumber}");
        }

        public async Task SendSmsAsync(string number, string message)
        {
            await SendSmsMessageWithPostMethodAsync(number, message, _smsUsername, _smsPassword, _smsSenderNumber);
        }
        private async Task<int> SendSmsMessageWithPostMethodAsync(string phone, string message, string username, string password,
            string senderPhoneNumber)
        {
            var resultCode = -1;

            var client = new HttpClient();
            var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"UserName", username},
                {"Password", password},
                {"PhoneNumber", senderPhoneNumber},
                {"MessageBody", message},
                {"RecNumber", phone},
                {"SmsClass", "1"},
            });

            var url = "https://RayganSMS.com/SendMessageWithPost.ashx";
            var response = client.PostAsync(url, formContent).Result;

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                logger.LogInformation($"SMS {message} sent to Number:{phone}");

                // resultCode = int.Parse(result);
            }
            else
            {
                throw new InternalServerException("خطایی در ارسال پیامک به وجود آمده است.");
            }

            return resultCode;
        }
    }
}