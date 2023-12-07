using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services;

public interface IEventServices:IScopedDependency
{
    Task<Event> CreateEventAsync(Event meeting,int userId);
    Task<Event> UpdateEventAsync(int id, Event meeting);
    Task DeleteEventAsync(int id);
    Task<Event> GetEventAsync(int eventId,int userId);
}