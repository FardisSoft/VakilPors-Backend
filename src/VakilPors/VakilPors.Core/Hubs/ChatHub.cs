using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;
using VakilPors.Core.Hubs.Clients;

namespace VakilPors.Core.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatClient>
    {
        public async Task SendMessage(ChatMessage message, IAppUnitOfWork appUnitOfWork)
        {
            await appUnitOfWork.ChatMessageRepo.AddAsync(message);
            await appUnitOfWork.SaveChangesAsync();
            await Clients.Group(message.ChatId.ToString()).ReceiveMessage(message);
        }
        public async Task ReadChatMessages(string chatId)
        {
            var userId = getUserId();
            await Clients.Group(chatId).ReadMessages();
        }

        public async Task AddToChat(string chatId, IAppUnitOfWork appUnitOfWork)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            var message = new ChatMessage()
            {
                ChatId = Convert.ToInt32(chatId),
                SenderId = getUserId(),
                Message = "من آنلاین شدم!",
            };
            await Clients.Group(chatId).ReceiveMessage(message);
            await appUnitOfWork.ChatMessageRepo.AddAsync(message);
            await appUnitOfWork.SaveChangesAsync();
        }
        protected string getPhoneNumber()
        {
            var phoneNumber = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (phoneNumber == null)
            {
                throw new NotFoundException("user not found");
            }
            return phoneNumber;
        }
        protected int getUserId()
        {
            var userId = Context.User.FindFirstValue("uid");
            if (userId == null)
            {
                throw new NotFoundException("user not found");
            }
            return Convert.ToInt32(userId);
        }
    }
}