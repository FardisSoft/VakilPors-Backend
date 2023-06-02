using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.User;
using static System.Net.WebRequestMethods;

namespace VakilPors.Core.Services
{
    public class TelegramService :ITelegramService
    {
        private readonly IAppUnitOfWork _appUnitOfWork;
        private readonly IMapper _mapper;

        public TelegramService(IAppUnitOfWork appUnitOfWork, IMapper mapper)
        {
            _appUnitOfWork = appUnitOfWork;
            _mapper = mapper;
        }

        
        public async Task SaveChatId(TelegramDto telegram)
        {
            var user = await _appUnitOfWork.UserRepo.AsQueryable().Where(x => x.PhoneNumber == telegram.phone_number).FirstOrDefaultAsync();
            user.Telegram = telegram.chat_id;
            await _appUnitOfWork.SaveChangesAsync();
        }

        public static Task SendToTelegram(string text, string chat_id)
        {
            string jsonData = "{\"chat_id\":\"" + chat_id + "\",\"text\":\"" + text + "\"}";
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var client = new HttpClient();
            var url = "https://fardissoft.pythonanywhere.com/post";
            var response = client.PostAsync(url, content);
            if (response.Result.IsSuccessStatusCode)
            {
                return Task.CompletedTask;
            }
            else
            {
                return Task.FromException(new Exception("Error in sending message to telegram"));
            }

            

        }
    }
}
