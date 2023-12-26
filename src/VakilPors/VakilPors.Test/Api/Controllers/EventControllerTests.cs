using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using VakilPors.Api.Controllers;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Event;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;

namespace VakilPors.Test.Api.Controllers;

public class EventControllerTests
{
    private readonly Mock<IEventServices> _mockEventServices = new Mock<IEventServices>();
    private readonly Mock<ILogger<EventController>> _mockLogger = new Mock<ILogger<EventController>>();
    private readonly EventController _controller;
    private readonly int _userId = 1; // Assumed test user ID

    public EventControllerTests()
    {
        _controller = new EventController(_mockEventServices.Object, _mockLogger.Object);
        SetUserControllerContext();
    }

    private void SetUserControllerContext()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal(_userId.ToString()) }
        };
    }

    private ClaimsPrincipal CreateClaimsPrincipal(string userIdValue)
    {
        var claims = new List<Claim> { new Claim("uid", userIdValue) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        return claimsPrincipal;
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedAtActionResult()
    {
        // Arrange
        var eventDto = new CreateEventDto
        {
            /* ... properties set up ... */
        };
        var createdEvent = new Event { Id = 3, /* ... properties set up ... */ };

        _mockEventServices.Setup(s => s.CreateEventAsync(eventDto, It.IsAny<int>()))
            .ReturnsAsync(createdEvent);

        // Act
        var result = await _controller.Create(eventDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        _mockEventServices.Verify(s => s.CreateEventAsync(eventDto, _userId), Times.Once());
        Assert.Equal(createdEvent, createdAtActionResult.Value);
        Assert.Equal(nameof(_controller.GetById), createdAtActionResult.ActionName);
        Assert.Equal(createdEvent.Id, createdAtActionResult.RouteValues["id"]);
    }

    // Tests for GetAll(), GetById(), GetGoogleCalendarEventById(), Update(), UpdateStatus(), and Delete() 
    // would follow a similar pattern, with adjustments to the asserts based on expected results 
    // and differences in the set up for each service method called.

    // Since Delete() is not actually implemented but throws an exception, 
    // for demonstration purposes, test that the exception is thrown.

    // [Fact]
    // public async Task Delete_ShouldThrowInvalidOperationException()
    // {
    //     // Arrange
    //     var id = _userId; // Using the same _userId assuming this is intended for testing purposes
    //
    //     // Act & Assert
    //     await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Delete(id));
    //     _mockEventServices.Verify(s => s.DeleteEventAsync(id, _userId), Times.Never());
    // }
    // ...

    [Fact]
    public async Task GetAll_ShouldReturnAllEventsForUser()
    {
        // Arrange
        var events = new List<Event>(); // Assuming Event is a class containing event details.
        _mockEventServices.Setup(s => s.GetEventsAsync(_userId))
            .ReturnsAsync(events);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(events, okResult.Value);
        _mockEventServices.Verify(s => s.GetEventsAsync(_userId), Times.Once());
    }

    [Fact]
    public async Task GetById_ShouldReturnEvent()
    {
        // Arrange
        int eventId = 2;
        var expectedEvent = new Event(); // Replace with actual event object expected
        _mockEventServices.Setup(s => s.GetEventAsync(eventId, _userId))
            .ReturnsAsync(expectedEvent);

        // Act
        var result = await _controller.GetById(eventId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedEvent, okResult.Value);
        _mockEventServices.Verify(s => s.GetEventAsync(eventId, _userId), Times.Once());
    }

    [Fact]
    public async Task GetGoogleCalendarEventById_ShouldReturnGoogleCalendarUrl()
    {
        // Arrange
        int eventId = 2;
        var expectedUrl = "http://google.com/calendar/event?details"; // Replace with actual URL expected
        _mockEventServices.Setup(s => s.GetGoogleCalendarUrl(eventId, _userId))
            .ReturnsAsync(expectedUrl);

        // Act
        var result = await _controller.GetGoogleCalendarEventById(eventId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedUrl, okResult.Value);
        _mockEventServices.Verify(s => s.GetGoogleCalendarUrl(eventId, _userId), Times.Once());
    }

    [Fact]
    public async Task Update_ShouldReturnUpdatedEvent()
    {
        // Arrange
        int eventId = 2;
        var eventToUpdate = new Event(); // Replace with actual event object used for update
        var updatedEvent = new Event(); // Replace with actual updated event object expected

        _mockEventServices.Setup(s => s.UpdateEventAsync(eventId, eventToUpdate, _userId))
            .ReturnsAsync(updatedEvent);

        // Act
        var result = await _controller.Update(eventId, eventToUpdate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(updatedEvent, okResult.Value);
        _mockEventServices.Verify(s => s.UpdateEventAsync(eventId, eventToUpdate, _userId), Times.Once());
    }

    [Fact]
    public async Task UpdateStatus_WhenUserIsVakil_ShouldUpdateEventStatus()
    {
        // Arrange
        int eventId = 2;
        var status = Status.ACCEPTED; // Assuming 'Status' is an enum
        var updatedEvent = new Event(); // Replace with actual updated event object expected

        _mockEventServices.Setup(s => s.UpdateEventStatusAsync(eventId, status, _userId))
            .ReturnsAsync(updatedEvent);

        // Mimic Authorize attribute by setting up a ClaimsPrincipal with the required role.
        SetUserControllerContextWithRole(RoleNames.Vakil);

        // Act
        var result = await _controller.UpdateStatus(eventId, status);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(updatedEvent, okResult.Value);
        _mockEventServices.Verify(s => s.UpdateEventStatusAsync(eventId, status, _userId), Times.Once());
    }

// ... additional setup method for role context ...
    private void SetUserControllerContextWithRole(string roleName)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, _userId.ToString()),
            new Claim(ClaimTypes.Role, roleName),
            new Claim("uid", 1.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    private void SetUserControllerContextWithUserId(string userIdValue)
    {
        var claims = new List<Claim> { new Claim("uid", userIdValue) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

// Tests here ...

    [Fact]
    public async Task Create_ValidUserId_ShouldCreateEvent()
    {
        // Arrange
        var eventDto = new CreateEventDto
        {
            /* ... set up properties ... */
        };
        var createdEvent = new Event { Id = 3 /* ... set up properties ... */ };

        _mockEventServices.Setup(s => s.CreateEventAsync(eventDto, _userId))
            .ReturnsAsync(createdEvent);

        // Act
        var result = await _controller.Create(eventDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(createdEvent, createdAtActionResult.Value);
        Assert.Equal(nameof(_controller.GetById), createdAtActionResult.ActionName);
        // Additional assertions as needed...
    }

// Since Delete() should throw an exception, we're not including a test for it here.
// The InvalidOperationException for Delete() method was already provided in the previous response.
}