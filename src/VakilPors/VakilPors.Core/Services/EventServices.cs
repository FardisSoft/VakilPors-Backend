using Microsoft.EntityFrameworkCore;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Event;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;

namespace VakilPors.Core.Services;

public class EventServices :IEventServices
{
    private readonly IAppUnitOfWork _uow;

    public EventServices(IAppUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Event> CreateEventAsync(CreateEventDto meeting,int userId)
    {
        if (await IsCollision(meeting.StartTime,meeting.EndTime))
        {
            throw new ApplicationException("Event time collides with an existing meeting.");
        }
        _
        await _uow.EventRepo.AddAsync(meeting);
        await _uow.SaveChangesAsync();

        return meeting;
    }
    public async Task<Event> GetEventAsync(int eventId,int userId)
    {
        var @event = await _uow.EventRepo.AsQueryableNoTracking()
            .Include(e=>e.Lawyer)
            .FirstOrDefaultAsync(e=>e.Id==eventId);
        if (@event == null)
        {
            throw new NotFoundException("Event not found.");
        }

        if (@event.UserId!=userId && @event.Lawyer.UserId!=userId)
        {
            throw new AccessViolationException("access denied.");
        }
        
        return @event;
    }

    public async Task<Event> UpdateEventAsync(int id, Event meeting)
    {
        var existingEvent = await _uow.EventRepo.FindAsync(id);
        if (existingEvent == null)
        {
            throw new NotFoundException("Event not found.");
        }

        if (await IsCollision(meeting.StartTime,meeting.EndTime, id))
        {
            throw new BadArgumentException("Event time collides with an existing meeting.");
        }

        existingEvent.StartTime = meeting.StartTime;
        existingEvent.EndTime = meeting.EndTime;
        existingEvent.Title = meeting.Title;
        existingEvent.Description = meeting.Description;

        await _uow.SaveChangesAsync();

        return existingEvent;
    }

    public async Task DeleteEventAsync(int id)
    {
        var meeting = await _uow.EventRepo.FindAsync(id);
        if (meeting == null)
        {
            throw new ApplicationException("Event not found.");
        }

        _uow.EventRepo.Remove(meeting);
        await _uow.SaveChangesAsync();
    }

    private async Task<bool> IsCollision(DateTime startTime,DateTime endTime, int? updatingEventId = null)
    {
        return await _uow.EventRepo.AsQueryable().AnyAsync(m =>
                   (updatingEventId == null || m.Id != updatingEventId) &&
                   ((startTime < m.EndTime && startTime >= m.StartTime)&&
                       (endTime > m.StartTime && endTime <= m.EndTime) &&
                       (startTime <= m.StartTime && endTime >= m.EndTime)));
    }
}