using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;
using VakilPors.Core.Hubs.Clients;

namespace VakilPors.Core.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatClient>
    {
        private readonly IAppUnitOfWork appUnitOfWork;
        private readonly ILogger<ChatHub> logger;

        public ChatHub(IAppUnitOfWork appUnitOfWork, ILogger<ChatHub> logger)
        {
            this.appUnitOfWork = appUnitOfWork;
            this.logger = logger;
        }
        public override Task OnConnectedAsync()
        {
            logger.LogInformation($"user with id:{getUserId()} connected  with connection id:{Context.ConnectionId}");
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            logger.LogInformation($"user with id:{getUserId()} disconnected with connection id:{Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(ChatMessage message)
        {
            await appUnitOfWork.ChatMessageRepo.AddAsync(message);
            await appUnitOfWork.SaveChangesAsync();
            message = await appUnitOfWork.ChatMessageRepo.AsQueryableNoTracking().Include(m => m.Sender).FirstOrDefaultAsync(m => m.Id == message.Id);
            logger.LogInformation($"user {message.Sender.Name} send message {message.Message} to chat id:{message.ChatId}");
            message.Sender.Messages = null;
            await Clients.Group(message.ChatId.ToString()).ReceiveMessage(message);
        }
        public async Task ReadChatMessages(string chatId)
        {
            var userId = getUserId();
            logger.LogInformation($"user with id:{userId} read messages of chat id:{chatId}");
            await Clients.Group(chatId).ReadMessages(chatId);
            var chatId_int = Convert.ToInt32(chatId);
            var messages = await appUnitOfWork.ChatMessageRepo.AsQueryable().Where(m => m.ChatId == chatId_int).ToArrayAsync();
            for (int i = 0; i < messages.Length; i++)
            {
                messages[i].IsRead = true;
            }
            await appUnitOfWork.SaveChangesAsync();
        }
        public async Task DeleteChatMessage(string chatId, string messageId)
        {
            var userId = getUserId();
            var chatId_int = Convert.ToInt32(chatId);
            var messageId_int = Convert.ToInt32(messageId);
            var message = await appUnitOfWork.ChatMessageRepo.FindAsync(messageId_int);
            logger.LogInformation($"user with id:{userId} delete message {message.Message} of chat id:{chatId}");
            if (message.SenderId != userId)
                throw new AccessViolationException("You do not have permission to perform this action");
            message.IsDeleted = true;
            await Clients.Group(chatId).DeleteMessage(message);
            await appUnitOfWork.SaveChangesAsync();
        }
        public async Task EditChatMessage(ChatMessage message)
        {
            var chatId = message.ChatId.ToString();
            var userId = getUserId();
            if (message.SenderId != userId)
            {
                var messageFromDb = await appUnitOfWork.ChatMessageRepo.FindAsync(message.Id);
                if (messageFromDb.IsCall)
                {
                    var chat = await appUnitOfWork.ChatRepo.AsQueryableNoTracking().Include(c => c.Users).FirstOrDefaultAsync(m => m.Id == message.ChatId);
                    if (!chat.Users.Any(u => u.Id == userId))
                    {
                        throw new AccessViolationException("You do not have permission to perform this action");
                    }
                }
                else
                {
                    throw new AccessViolationException("You do not have permission to perform this action");
                }

            }
            logger.LogInformation($"user with id:{userId} edited message {message.Message} of chat id:{chatId}");
            message.IsEdited = true;
            await Clients.Group(chatId).EditMessage(message);
            appUnitOfWork.ChatMessageRepo.Update(message);
            await appUnitOfWork.SaveChangesAsync();
        }

        public async Task AddToChat(string chatId)
        {
            logger.LogInformation($"user with id:{getUserId()} edited joined chat with id:{chatId} with connection id:{Context.ConnectionId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            // var message = new ChatMessage()
            // {
            //     ChatId = Convert.ToInt32(chatId),
            //     SenderId = getUserId(),
            //     Message = "من آنلاین شدم!",
            // };
            // await Clients.Group(chatId).ReceiveMessage(message);
            // await appUnitOfWork.ChatMessageRepo.AddAsync(message);
            // await appUnitOfWork.SaveChangesAsync();
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