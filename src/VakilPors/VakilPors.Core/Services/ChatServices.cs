using Microsoft.EntityFrameworkCore;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Entities;

namespace VakilPors.Core.Services
{
    public class ChatServices : IChatServices
    {
        private readonly IAppUnitOfWork appUnitOfWork;

        public ChatServices(IAppUnitOfWork appUnitOfWork)
        {
            this.appUnitOfWork = appUnitOfWork;
        }

        public async Task<Chat> CreateChat(int userId1, int userId2)
        {
            var chat = await appUnitOfWork.ChatRepo.AsQueryableNoTracking().Where(c => c.Users.Select(u => u.Id).Contains(userId1) && c.Users.Select(u => u.Id).Contains(userId2)).FirstOrDefaultAsync();
            if (chat is null)
            {
                chat = new Chat()
                {
                    Users = new List<User>() {
                        new User() { Id = userId1},
                        new User() { Id = userId2 }
                    },
                };
            }
            await appUnitOfWork.ChatRepo.AddAsync(chat);
            await appUnitOfWork.SaveChangesAsync();
            return chat;
        }

        public async Task<ICollection<Chat>> GetChatsOfUser(int userId)
        {
            return await appUnitOfWork.ChatRepo.AsQueryableNoTracking().Where(c => c.Users.Select(u => u.Id).Contains(userId)).ToArrayAsync();
        }

        public async Task<ICollection<Chat>> GetChatsWithMessagesOfUser(int userId)
        {
            return await appUnitOfWork.ChatRepo.AsQueryableNoTracking()
            .Include(c => c.ChatMessages)
            .ThenInclude(m => m.Sender)
            .Where(c => c.Users.Select(u => u.Id).Contains(userId)).ToArrayAsync();
        }

        public async Task<ICollection<ChatMessage>> GetMessagesOfChat(int userId, int chatId)
        {
            var chat = await appUnitOfWork.ChatRepo.AsQueryableNoTracking().Include(c => c.ChatMessages).ThenInclude(m => m.Sender).FirstOrDefaultAsync(c => c.Id == chatId);
            if (!chat.Users.Select(u => u.Id).Contains(userId))
                throw new AccessViolationException("You do not have permission to perform this action");
            return chat.ChatMessages;
        }
    }
}