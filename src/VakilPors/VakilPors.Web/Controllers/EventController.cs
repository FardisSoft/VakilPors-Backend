using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Event;
using VakilPors.Core.Domain.Dtos.Ocr;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Response;
using VakilPors.Web.Controllers;

namespace VakilPors.Api.Controllers;

[ApiController]
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Event meeting)
    {
        var updatedEvent = await _eventServices.UpdateEventAsync(id, meeting);
        return Ok(updatedEvent);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _eventServices.DeleteEventAsync(id);
        return NoContent();
    }
}