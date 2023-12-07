using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Event;
using VakilPors.Core.Domain.Entities;
using VakilPors.Web.Controllers;

namespace VakilPors.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class EventController : MyControllerBase
{
    private readonly IEventServices _eventServices;
    private readonly ILogger<EventController> _logger;

    public EventController(IEventServices eventServices, ILogger<EventController> logger)
    {
        _eventServices = eventServices;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEventDto eventDto)
    {
        var createdEvent = await _eventServices.CreateEventAsync(eventDto,getUserId());
        return CreatedAtAction(nameof(GetById), new { id = createdEvent.Id }, createdEvent);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var meeting = await _eventServices.GetEventAsync(id, getUserId());
        return Ok(meeting);
    }
    [HttpGet("/google-calendar/{id}")]
    public async Task<IActionResult> GetGoogleCalendarEventById(int id)
    {
        var meeting = await _eventServices.GetGoogleCalendarUrl(id, getUserId());
        return Ok(meeting);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Event meeting)
    {
        var updatedEvent = await _eventServices.UpdateEventAsync(id, meeting, getUserId());
        return Ok(updatedEvent);
    }
    [HttpPatch("/status/{id}")]
    [Authorize(Roles = RoleNames.Vakil)]
    public async Task<IActionResult> UpdateStatus(int id, Status status)
    {
        var updatedEvent = await _eventServices.UpdateEventStatusAsync(id, status, getUserId());
        return Ok(updatedEvent);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        throw new InvalidOperationException("Not Meant to be done!");
        await _eventServices.DeleteEventAsync(id, getUserId());
        return NoContent();
    }
}