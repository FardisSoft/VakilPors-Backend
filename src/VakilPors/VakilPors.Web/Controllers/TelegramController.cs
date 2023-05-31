using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.User;

namespace VakilPors.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TelegramController :MyControllerBase
    {
        private readonly ITelegramService _telegramService;

        public TelegramController(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }
        [HttpPost]
        [Route("SaveChatId")]
        public async Task SaveChatId([FromBody] TelegramDto telegram)
        {
            await _telegramService.SaveChatId(telegram);

        }
    }
}
