namespace VakilPors.Core.Hubs.Clients;

public interface IMeetingClient
{
    Task UserConnected(string userId);
    Task UserDisconnected(string userId);
}
