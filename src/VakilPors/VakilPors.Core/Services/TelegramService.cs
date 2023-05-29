using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.User;

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
            var user = await _appUnitOfWork.UserRepo.FindAsync(telegram.phone_number);
            user.Telegram = telegram.chat_id;
            await _appUnitOfWork.SaveChangesAsync();
        }
    }
}
