using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services
{
    public interface ITelegramService :IScopedDependency
    {
        //Task SaveChatId(string phone_number, string chat_id);
        Task SaveChatId(TelegramDto telegram);

    }
}
