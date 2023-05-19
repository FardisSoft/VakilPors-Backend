using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Hubs.Clients;

namespace VakilPors.Core.Hubs;

public class MeetingHub : Hub<IMeetingClient>
{
    private readonly IAppUnitOfWork appUnitOfWork;
    private readonly ILogger<MeetingHub> logger;

    public MeetingHub(IAppUnitOfWork appUnitOfWork, ILogger<MeetingHub> logger)
    {
        this.appUnitOfWork = appUnitOfWork;
        this.logger = logger;
    }
    public async Task JoinMeeting(string meetingId, string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, meetingId);
        await Clients.Group(meetingId).UserConnected(userId);
    }

}
