using Microsoft.EntityFrameworkCore;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Entities;

namespace VakilPors.Core.Services
{
    public class ChatServices : IChatServices
    {
        private readonly IAppUnitOfWork appUnitOfWork;
        private readonly IPremiumService _premiumService;

        public ChatServices(IAppUnitOfWork appUnitOfWork, IPremiumService premiumService)
        {
            this.appUnitOfWork = appUnitOfWork;
            _premiumService = premiumService;
        }

        public async Task<Chat> CreateChat(int userId1, int userId2)
        {
            var chat = await appUnitOfWork.ChatRepo.AsQueryableNoTracking().Where(c => c.Users.Select(u => u.Id).Contains(userId1) && c.Users.Select(u => u.Id).Contains(userId2)).FirstOrDefaultAsync();
            if (chat is null)
            {
                chat = new Chat()
                {
                    Users = new List<User>() {
                        await appUnitOfWork.UserRepo.FindAsync(userId1),
                        await appUnitOfWork.UserRepo.FindAsync(userId2),
                    },
                };
                await appUnitOfWork.ChatRepo.AddAsync(chat);

                var lawyer1 = await appUnitOfWork.LawyerRepo.AsQueryable().FirstOrDefaultAsync(x => x.UserId == userId1);
                if (lawyer1 != null)
                {
                    lawyer1.Tokens += 1;
                    appUnitOfWork.LawyerRepo.Update(lawyer1);
                }

                var lawyer2 = await appUnitOfWork.LawyerRepo.AsQueryable().FirstOrDefaultAsync(x => x.UserId == userId2);
                {
                    lawyer2.Tokens += 1;
                    appUnitOfWork.LawyerRepo.Update(lawyer2);
                }

                await appUnitOfWork.SaveChangesAsync();
            }

            return chat;
        }

        public async Task<ICollection<Chat>> GetChatsOfUser(int userId)
        {
            var chats = await appUnitOfWork.ChatRepo
                .AsQueryableNoTracking()
                .Include(c => c.Users)
                .Where(c => c.Users.Select(u => u.Id).Contains(userId))
                .ToArrayAsync();

            chats = chats
                .OrderByDescending(x => x.Users
                    .Where(u => u.Id != userId)
                    .Any(u => _premiumService.DoseUserHaveAnyActiveSubscription(u.Id).Result)).ToArray();

            return chats;
        }

        public async Task<ICollection<Chat>> GetChatsWithMessagesOfUser(int userId)
        {
            var chats = await appUnitOfWork.ChatRepo.AsQueryableNoTracking()
            .Include(c => c.Users)
            .Include(c => c.ChatMessages)
            .ThenInclude(m => m.Sender)
            .Include(c => c.ChatMessages)
            .ThenInclude(m => m.ReplyMessage)
            .Where(c => c.Users.Select(u => u.Id).Contains(userId)).ToArrayAsync();

            chats = chats
                .OrderByDescending(x => x.Users
                    .Where(u => u.Id != userId)
                    .Any(u => _premiumService.DoseUserHaveAnyActiveSubscription(u.Id).Result)).ToArray();

            return chats;
        }

        public async Task<ICollection<ChatMessage>> GetMessagesOfChat(int userId, int chatId)
        {
            var chat = await appUnitOfWork.ChatRepo.AsQueryableNoTracking()
            .Include(c => c.ChatMessages)
            .ThenInclude(m => m.Sender)
            .Include(c => c.ChatMessages)
            .ThenInclude(m => m.ReplyMessage)
            .FirstOrDefaultAsync(c => c.Id == chatId);
            if (!chat.Users.Select(u => u.Id).Contains(userId))
                throw new AccessViolationException("You do not have permission to perform this action");
            return chat.ChatMessages;
        }
    }
}