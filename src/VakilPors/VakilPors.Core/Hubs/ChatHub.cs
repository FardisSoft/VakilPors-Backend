using Microsoft.AspNetCore.SignalR;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Hubs.Clients;

namespace VakilPors.Core.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {

        public async Task SendMessage(ChatMessage message)
        {
            await Clients.All.ReceiveMessage(message);
        }
    }
}