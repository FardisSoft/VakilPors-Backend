using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VakilPors.Core.Contracts.Services;

namespace VakilPors.Core.Services
{
    public class SMSSender : ISMSSender
    {
        private string _smsSenderNumber;
        private string _smsUsername;
        private string _smsPassword;

        public SMSSender(IConfiguration configuration)
        {
            _smsSenderNumber= Environment.GetEnvironmentVariable("RAYGAN_SMS_SENDER_NUMBER",EnvironmentVariableTarget.User) ?? configuration["RAYGAN_SMS:SENDER_NUMBER"];
            _smsUsername= Environment.GetEnvironmentVariable("RAYGAN_SMS_USERNAME",EnvironmentVariableTarget.User) ?? configuration["RAYGAN_SMS:USERNAME"];
            _smsPassword= Environment.GetEnvironmentVariable("RAYGAN_SMS_PASSWORD",EnvironmentVariableTarget.User) ?? configuration["RAYGAN_SMS:PASSWORD"];
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
                 resultCode = int.Parse(result);
            }

            return resultCode;
        }
    }
}