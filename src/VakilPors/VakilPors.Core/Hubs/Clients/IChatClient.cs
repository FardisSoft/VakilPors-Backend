using VakilPors.Core.Domain.Entities;

namespace VakilPors.Core.Hubs.Clients;

public interface IChatClient
{
    Task ReceiveMessage(ChatMessage message);

}