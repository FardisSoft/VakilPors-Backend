using VakilPors.Core.Domain.Dtos.Event;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services;

public interface IEventServices:IScopedDependency
{
    Task<Event> CreateEventAsync(CreateEventDto eventDto,int userId);
    Task<Event> UpdateEventAsync(int id, Event @event, int userId);
    Task<Event> UpdateEventStatusAsync(int id, Status status, int userId);
    Task DeleteEventAsync(int id, int userId);
    Task<Event> GetEventAsync(int eventId,int userId);
    Task<string> GetGoogleCalendarUrl(int id,int userId);
    Task<List<Event>> GetEventsAsync(int userId);
}