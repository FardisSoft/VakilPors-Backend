using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
        public async Task ReadChatMessages(string chatId, IAppUnitOfWork appUnitOfWork)
        {
            await Clients.Group(chatId).ReadMessages();
            var chatId_int = Convert.ToInt32(chatId);
            var userId = getUserId();
            var messages = await appUnitOfWork.ChatMessageRepo.AsQueryable().Where(m => m.ChatId == chatId_int).ToArrayAsync();
            for (int i = 0; i < messages.Length; i++)
            {
                messages[i].IsRead = true;
            }
            await appUnitOfWork.SaveChangesAsync();
        }
        public async Task DeleteChatMessage(string chatId, string messageId, IAppUnitOfWork appUnitOfWork)
        {
            var chatId_int = Convert.ToInt32(chatId);
            var messageId_int = Convert.ToInt32(messageId);
            var message = await appUnitOfWork.ChatMessageRepo.FindAsync(messageId_int);
            var userId = getUserId();
            if (message.SenderId != userId)
                throw new AccessViolationException("You do not have permission to perform this action");
            message.IsDeleted = true;
            await Clients.Group(chatId).DeleteMessage(messageId);
            await appUnitOfWork.SaveChangesAsync();
        }
        public async Task EditChatMessage(ChatMessage message, IAppUnitOfWork appUnitOfWork)
        {
            var chatId = message.ChatId.ToString();
            var userId = getUserId();
            if (message.SenderId != userId)
                throw new AccessViolationException("You do not have permission to perform this action");
            message.IsEdited = true;
            await Clients.Group(chatId).EditMessage(message);
            appUnitOfWork.ChatMessageRepo.Update(message);
            await appUnitOfWork.SaveChangesAsync();
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