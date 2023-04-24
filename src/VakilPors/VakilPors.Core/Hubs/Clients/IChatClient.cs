using VakilPors.Core.Domain.Entities;

namespace VakilPors.Core.Hubs.Clients;

public interface IChatClient
{
    Task ReceiveMessage(ChatMessage message);
    Task ReadMessages();
    Task DeleteMessage(string messageId);
    Task EditMessage(ChatMessage message);

}