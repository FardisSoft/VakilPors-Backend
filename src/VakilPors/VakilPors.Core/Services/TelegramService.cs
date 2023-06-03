using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Core.Exceptions;
using static System.Net.WebRequestMethods;

namespace VakilPors.Core.Services
{
    public class TelegramService :ITelegramService
    {
        private readonly IAppUnitOfWork _appUnitOfWork;
        private readonly IMapper _mapper;
        public static  ILogger<TelegramService> _logger;
        public TelegramService(IAppUnitOfWork appUnitOfWork, IMapper mapper, ILogger<TelegramService> logger)
        {
            _appUnitOfWork = appUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        
        public async Task SaveChatId(TelegramDto telegram)
        {
            var user = await _appUnitOfWork.UserRepo.AsQueryable().Where(x => x.PhoneNumber == telegram.phone_number).FirstOrDefaultAsync();
            user.Telegram = telegram.chat_id;
            await _appUnitOfWork.SaveChangesAsync();
        }

        public async static Task SendToTelegram(string text, string chat_id)
        {
            try
            {
                string jsonData = "{\"chat_id\":\"" + chat_id + "\",\"text\":\"" + text + "\"}";
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                var url = "https://fardissoft.pythonanywhere.com/post";
                var response = await client.PostAsync(url, content);
            }
            catch (System.Exception)
            {
                //throw new BadArgumentException("Error in sending message to telegram");
                _logger.LogError("Error in sending message to telegram");
            }
            //if (!response.IsSuccessStatusCode)
            //{
            //    throw new BadArgumentException("Error in sending message to telegram");
            //}
            

            

        }
    }
}
