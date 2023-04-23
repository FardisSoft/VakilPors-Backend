using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services
{
    public interface IChatServices : IScopedDependency
    {
        Task<ICollection<Chat>> GetChatsOfUser(int userId);
        Task<ICollection<Chat>> GetChatsWithMessagesOfUser(int userId);
        Task<ICollection<ChatMessage>> GetMessagesOfChat(int userId, int chatId);
        Task<Chat> CreateChat(int userId1, int userId2);
    }
}