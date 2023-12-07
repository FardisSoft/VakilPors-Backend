using AutoMapper;
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
    private readonly IMapper _mapper;

    public EventServices(IAppUnitOfWork uow,IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Event> CreateEventAsync(CreateEventDto createEventDto,int userId)
    {
        if (await HasCollision(createEventDto.StartTime,createEventDto.EndTime))
        {
            throw new BadArgumentException("Event time collides with an existing event.");
        }

        var @event = _mapper.Map<Event>(createEventDto);
        await _uow.EventRepo.AddAsync(@event);
        await _uow.SaveChangesAsync();

        return @event;
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

    public async Task<Event> UpdateEventAsync(int id, Event @event,int userId)
    {
        var existingEvent = await _uow.EventRepo.AsQueryableNoTracking()
            .Include(e=>e.Lawyer)
            .FirstOrDefaultAsync(e=>e.Id==id);
        if (existingEvent == null)
        {
            throw new NotFoundException("Event not found.");
        }
        if (existingEvent.UserId!=userId && existingEvent.Lawyer.UserId!=userId)
        {
            throw new AccessViolationException("access denied.");
        }

        if (await HasCollision(@event.StartTime,@event.EndTime, id))
        {
            throw new BadArgumentException("Event time collides with an existing event.");
        }

        existingEvent.StartTime = @event.StartTime;
        existingEvent.EndTime = @event.EndTime;
        existingEvent.Title = @event.Title;
        existingEvent.Description = @event.Description;
        existingEvent.Status = existingEvent.Lawyer.UserId==userId ? @event.Status : Status.PENDING;

        await _uow.SaveChangesAsync();

        return existingEvent;
    }

    public async Task<Event> UpdateEventStatusAsync(int id, Status status,int userId)
    {
        var existingEvent = await _uow.EventRepo.AsQueryableNoTracking()
            .Include(e=>e.Lawyer)
            .FirstOrDefaultAsync(e=>e.Id==id);
        if (existingEvent == null)
        {
            throw new NotFoundException("Event not found.");
        }
        if (existingEvent.Lawyer.UserId!=userId)
        {
            throw new AccessViolationException("access denied.");
        }

        existingEvent.Status = status;
        await _uow.SaveChangesAsync();

        return existingEvent;
    }

    public async Task DeleteEventAsync(int id,int userId)
    {
        var @event = await _uow.EventRepo.AsQueryableNoTracking()
            .Include(e=>e.Lawyer)
            .FirstOrDefaultAsync(e=>e.Id==id);
        if (@event == null)
        {
            throw new ApplicationException("Event not found.");
        }
        if (@event.Lawyer.UserId!=userId)
        {
            throw new AccessViolationException("access denied.");
        }

        _uow.EventRepo.Remove(@event);
        await _uow.SaveChangesAsync();
    }
    public async Task<string> GetGoogleCalendarUrl(int id,int userId)
    {
        var myEvent = await GetEventAsync(id,userId);
        string startTimeFormatted = myEvent.StartTime.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
        string endTimeFormatted = myEvent.EndTime.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
    
        string url = $"https://www.google.com/calendar/render?action=TEMPLATE&text={Uri.EscapeDataString(myEvent.Title)}&dates={startTimeFormatted}/{endTimeFormatted}&details={Uri.EscapeDataString(myEvent.Description)}&sf=true&output=xml&ctz=Asia/Tehran";

        return url;
    }

    private async Task<bool> HasCollision(DateTime startTime,DateTime endTime, int? updatingEventId = null)
    {
        return await _uow.EventRepo.AsQueryable().Where(m=>m.Status==Status.ACCEPTED).AnyAsync(m =>
                   (updatingEventId == null || m.Id != updatingEventId) &&
                   ((startTime < m.EndTime && startTime >= m.StartTime)&&
                       (endTime > m.StartTime && endTime <= m.EndTime) &&
                       (startTime <= m.StartTime && endTime >= m.EndTime)));
    }
}