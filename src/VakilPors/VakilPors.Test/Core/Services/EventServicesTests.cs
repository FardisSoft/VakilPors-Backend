using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using MockQueryable.Moq;
using Moq;
using VakilPors.Contracts.Repositories;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Domain.Dtos.Event;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;
using VakilPors.Core.Services;

namespace VakilPors.Test.Core.Services;

public class EventServicesTests
{
    private readonly Mock<IAppUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly EventServices _eventServices;

    public EventServicesTests()
    {
        _mockUnitOfWork = new Mock<IAppUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _eventServices = new EventServices(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateEventAsync_ShouldCreateEventWhenNoCollision()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IAppUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var eventServices = new EventServices(mockUnitOfWork.Object, mockMapper.Object);

        var createEventDto = new CreateEventDto
        {
            // Populated properties for CreateEventDto
        };
        int userId = 1;
        var eventEntity = new Event();

        // Setting up the mock to return false for HasCollision, meaning no collision detected
        // eventServices.GetType().GetMethod("HasCollision", BindingFlags.NonPublic)
        //     .Invoke(eventServices, new object[] { createEventDto.StartTime, createEventDto.EndTime, null })
        //     .Returns(false);
        var events = new List<Event> { }.BuildMock();
        var mockEventRepo = new Mock<IGenericRepo<Event>>();
        mockEventRepo.Setup(repo => repo.AsQueryable()).Returns(events);
        mockUnitOfWork.Setup(uow => uow.EventRepo).Returns(mockEventRepo.Object);

        mockMapper.Setup(m => m.Map<Event>(It.IsAny<CreateEventDto>())).Returns(eventEntity);
        mockUnitOfWork.Setup(uow => uow.EventRepo.AddAsync(It.IsAny<Event>())).Verifiable();
        mockUnitOfWork.Setup(uow => uow.SaveChangesAsync()).Verifiable();

        // Act
        var result = await eventServices.CreateEventAsync(createEventDto, userId);

        // Assert
        mockUnitOfWork.Verify(uow => uow.EventRepo.AddAsync(It.IsAny<Event>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId); // Ensure the UID was set correctly
    }

    [Fact]
    public async Task GetEventAsync_ShouldReturnEventForValidUser()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IAppUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var eventServices = new EventServices(mockUnitOfWork.Object, mockMapper.Object);

        var eventId = 123;
        var userId = 1;
        var expectedEvent = new Event
        {
            Id = eventId,
            UserId = userId,
            // Other property initializations
        };

        var events = new List<Event> { expectedEvent }.BuildMock();
        var mockEventRepo = new Mock<IGenericRepo<Event>>();
        mockEventRepo.Setup(repo => repo.AsQueryableNoTracking()).Returns(events);
        mockUnitOfWork.Setup(uow => uow.EventRepo).Returns(mockEventRepo.Object);

        // Act
        var actualEvent = await eventServices.GetEventAsync(eventId, userId);

        // Assert
        Assert.NotNull(actualEvent);
        Assert.Equal(eventId, actualEvent.Id);
        Assert.Equal(userId, actualEvent.UserId);
    }

    [Fact]
    public async Task GetEventsAsync_ShouldReturnEventForValidUser()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IAppUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var eventServices = new EventServices(mockUnitOfWork.Object, mockMapper.Object);

        var eventId = 123;
        var userId = 1;
        var expectedEvent = new Event
        {
            Id = eventId,
            UserId = userId,
            // Other property initializations
        };

        var events = new List<Event> { expectedEvent }.BuildMock();
        var mockEventRepo = new Mock<IGenericRepo<Event>>();
        mockEventRepo.Setup(repo => repo.AsQueryableNoTracking()).Returns(events);
        mockUnitOfWork.Setup(uow => uow.EventRepo).Returns(mockEventRepo.Object);

        // Act
        var actualEvent = await eventServices.GetEventsAsync(userId);

        // Assert
        Assert.NotNull(actualEvent);
        Assert.Equal(eventId, actualEvent[0].Id);
        Assert.Equal(userId, actualEvent[0].UserId);
    }

    [Fact]
    public async Task DeleteEventAsync_ShouldDeleteEventForAuthorizedUser()
    {
        // Arrange
        var eventId = 1;
        var userId = 1;


        var @event = new Event { Id = eventId, Lawyer = new Lawyer { UserId = userId } };

        // Use a list to mock async operations like ToListAsync
        var events = new List<Event> { @event }.BuildMock();

        _mockUnitOfWork.Setup(uow => uow.EventRepo.AsQueryableNoTracking()).Returns(events);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync()).Verifiable();

        // Act
        await _eventServices.DeleteEventAsync(eventId, userId);

        // Assert
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once());
    }

    [Fact]
    public async Task DeleteEventAsync_ShouldThrowAccessViolationExceptionForUnauthorizedUser()
    {
        // Arrange
        var eventId = 1;
        var userId = 1;
        var unauthorizedUserId = 2;


        var @event = new Event { Id = eventId, Lawyer = new Lawyer { UserId = userId } };

        var events = new List<Event> { @event }.BuildMock();


        _mockUnitOfWork.Setup(uow => uow.EventRepo.AsQueryableNoTracking()).Returns(events);

        // Act & Assert
        await Assert.ThrowsAsync<AccessViolationException>(() =>
            _eventServices.DeleteEventAsync(eventId, unauthorizedUserId));
    }

    [Fact]
    public async Task GetGoogleCalendarUrl_ShouldReturnCorrectUrl()
    {
        // Arrange
        var eventId = 1;
        var userId = 1;
        var eventTitle = "Test Event";
        var eventDescription = "Event Description";
        var startTime = new DateTime(2023, 3, 10, 14, 30, 0, DateTimeKind.Utc);
        var endTime = new DateTime(2023, 3, 10, 15, 0, 0, DateTimeKind.Utc);
        var expectedStartTime = "20230310T143000Z"; // Assuming a Tehran Time Zone of +03:30
        var expectedEndTime = "20230310T150000Z"; // Assuming a Tehran Time Zone of +03:30

        var myEvent = new Event
        {
            Id = eventId,
            UserId = userId,
            Title = eventTitle,
            Description = eventDescription,
            StartTime = startTime,
            EndTime = endTime,
        };

        var events = new List<Event> { myEvent }.BuildMock();


        _mockUnitOfWork.Setup(uow => uow.EventRepo.AsQueryableNoTracking()).Returns(events);
        // Act
        var actualUrl = await _eventServices.GetGoogleCalendarUrl(eventId, userId);

        // Assert
        var expectedUrl =
            $"https://www.google.com/calendar/render?action=TEMPLATE&text={Uri.EscapeDataString(eventTitle)}&dates={expectedStartTime}/{expectedEndTime}&details={Uri.EscapeDataString(eventDescription)}&sf=true&output=xml&ctz=Asia/Tehran";
        Assert.Equal(expectedUrl, actualUrl);
    }

    [Fact]
    public async Task UpdateEventAsync_ShouldUpdateEventSuccessfully()
    {
        // Arrange
        var existingEvent = new Event
        {
            Id = 1,
            Lawyer = new Lawyer { UserId = 1 },
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2),
            Status = Status.PENDING
        };
        var events = new List<Event> { existingEvent }.BuildMock();


        _mockUnitOfWork.Setup(uow => uow.EventRepo.AsQueryableNoTracking()).Returns(events);
        _mockUnitOfWork.Setup(uow => uow.EventRepo.AsQueryable()).Returns(events);

        var updateEvent = new Event
        {
            StartTime = DateTime.Now.AddHours(3),
            EndTime = DateTime.Now.AddHours(5),
            Title = "Updated Title",
            Description = "Updated Description"
        };
        int userId = 1;


        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _eventServices.UpdateEventAsync(existingEvent.Id, updateEvent, userId);

        // Assert
        Assert.Equal(updateEvent.Title, result.Title);
        Assert.Equal(updateEvent.Description, result.Description);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateEventAsync_ShouldThrowNotFoundException()
    {
        // Arrange
        var events = new List<Event> { }.BuildMock();


        _mockUnitOfWork.Setup(uow => uow.EventRepo.AsQueryableNoTracking()).Returns(events);

        var updateEvent = new Event();
        int userId = 1;
        int eventId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _eventServices.UpdateEventAsync(eventId, updateEvent, userId));
    }

    [Theory]
    [InlineData(Status.ACCEPTED)]
    [InlineData(Status.REJECTED)]
    public async Task UpdateEventStatusAsync_ShouldUpdateStatus(Status newStatus)
    {
        // Arrange
        var eventId = 1;
        var userId = 1;
        var eventMock = new Event { Id = eventId, Lawyer = new Lawyer { UserId = userId }, Status = Status.PENDING };
        var data = new List<Event> { eventMock }.BuildMock();

        _mockUnitOfWork.Setup(u => u.EventRepo.AsQueryable())
            .Returns(data);

        // Act
        var updatedEvent = await _eventServices.UpdateEventStatusAsync(eventId, newStatus, userId);

        // Assert
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once());
        Assert.Equal(newStatus, updatedEvent.Status);
    }

    [Fact]
    public async Task UpdateEventStatusAsync_ShouldThrowNotFoundException_WhenEventDoesNotExist()
    {
        // Arrange
        var eventIdNotInDb = 2;
        var userId = 1;
        var events = new List<Event>().BuildMock(); // No events in the mock database

        _mockUnitOfWork.Setup(u => u.EventRepo.AsQueryable())
            .Returns(events);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _eventServices.UpdateEventStatusAsync(eventIdNotInDb, Status.ACCEPTED, userId)
        );
    }

    [Fact]
    public async Task UpdateEventStatusAsync_ShouldThrowAccessViolationException_WhenUserIsNotLawyer()
    {
        // Arrange
        var eventId = 1;
        var eventOwnerId = 1; // Event is owned by user with ID 1
        var userIdTryingToUpdate = 2; // Different user who will attempt to update
        var eventMock = new Event { Id = eventId, Lawyer = new Lawyer { UserId = eventOwnerId } };
        var data = new List<Event> { eventMock }.BuildMock();

        _mockUnitOfWork.Setup(u => u.EventRepo.AsQueryable())
            .Returns(data);

        // Act & Assert
        await Assert.ThrowsAsync<AccessViolationException>(
            () => _eventServices.UpdateEventStatusAsync(eventId, Status.ACCEPTED, userIdTryingToUpdate)
        );
    }

    [Fact]
    public async Task GetEventAsync_ShouldThrowNotFoundException_WhenEventDoesNotExist()
    {
        // Arrange
        int nonExistentEventId = 999;
        int userId = 1;
        var events = new List<Event>().BuildMock(); // Empty list to simulate no events

        _mockUnitOfWork.Setup(u => u.EventRepo.AsQueryable()).Returns(events);
        _mockUnitOfWork.Setup(u => u.EventRepo.AsQueryableNoTracking()).Returns(events);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _eventServices.GetEventAsync(nonExistentEventId, userId)
        );
    }

    [Fact]
    public async Task GetEventAsync_ShouldThrowAccessViolationException_WhenUserIsNotAssociated()
    {
        // Arrange
        var eventId = 1;
        var userId = 1; // The user who will attempt to retrieve the event
        var lawyerId = 2; // Different userId to signify the lawyer who owns the event

        var existingEvent = new Event { Id = eventId, UserId = lawyerId, Lawyer = new Lawyer { UserId = lawyerId } };
        var events = new List<Event> { existingEvent }.BuildMock();

        _mockUnitOfWork.Setup(u => u.EventRepo.AsQueryable()).Returns(events);
        _mockUnitOfWork.Setup(u => u.EventRepo.AsQueryableNoTracking()).Returns(events);

        // Act & Assert
        await Assert.ThrowsAsync<AccessViolationException>(
            () => _eventServices.GetEventAsync(eventId, userId)
        );
    }

    [Fact]
    public async Task DeleteEventAsync_ShouldThrowApplicationException_WhenEventNotFound()
    {
        // Arrange
        var userId = 1;
        var eventIdNotInDb = 999; // This event ID does not exist
        var events = new List<Event>().BuildMock(); // Empty list to simulate no events

        _mockUnitOfWork.Setup(u => u.EventRepo.AsQueryable()).Returns(events);
        _mockUnitOfWork.Setup(u => u.EventRepo.AsQueryableNoTracking()).Returns(events);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationException>(
            () => _eventServices.DeleteEventAsync(eventIdNotInDb, userId)
        );

        Assert.Equal("Event not found.", exception.Message);
    }

    [Fact]
    public async Task DeleteEventAsync_ShouldThrowAccessViolationException_WhenUserNotLawyer()
    {
        // Arrange
        var eventId = 1;
        var eventLawyerUserId = 2; // Lawyer's user ID for the event
        var requestingUserId = 3; // The user ID of the requester who is not the lawyer
        var @event = new Event { Id = eventId, Lawyer = new Lawyer { UserId = eventLawyerUserId } };
        var events = new List<Event> { @event }.BuildMock();

        _mockUnitOfWork.Setup(u => u.EventRepo.AsQueryable()).Returns(events);
        _mockUnitOfWork.Setup(u => u.EventRepo.AsQueryableNoTracking()).Returns(events);
        // Assuming your repository sets up the include
        // _mockEventSet.Setup(m => m.Include(It.IsAny<string>())).Returns(_mockEventSet.Object); 

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AccessViolationException>(
            () => _eventServices.DeleteEventAsync(eventId, requestingUserId)
        );

        Assert.Equal("access denied.", exception.Message);
    }


    [Fact]
    public async Task CreateEventAsync_ShouldThrowBadArgumentException_WhenEventTimeCollides()
    {
        // Arrange
        var userId = 1;
        var startTime = new DateTime(2023, 04, 01, 10, 00, 00);
        var endTime = new DateTime(2023, 04, 01, 12, 00, 00);
        var createEventDto = new CreateEventDto
        {
            // Populate with necessary data, including the times that would result in collision
            StartTime = startTime,
            EndTime = endTime
        };
        var @event = new Event { Id = 1, StartTime = startTime, EndTime = endTime, Status = Status.ACCEPTED };
        var events = new List<Event> { @event }.BuildMock();

        // Simulate that there is already an event within the time period we're testing
        _mockUnitOfWork.Setup(u => u.EventRepo.AsQueryable()).Returns(events);
        _mockUnitOfWork.Setup(u => u.EventRepo.AsQueryableNoTracking()).Returns(events);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadArgumentException>(
            () => _eventServices.CreateEventAsync(createEventDto, userId)
        );

        Assert.Equal("Event time collides with an existing event.", exception.Message);
    }

    [Fact]
    public async Task UpdateEventAsync_ShouldThrowNotFoundException_ForNonExistentEvent()
    {
        // Arrange
        var eventId = 1;
        var userId = 1;
        // _mockEventSet.Setup(m => m.Include(It.IsAny<Expression<Func<Event, object>>>()))
        //     .Returns(_mockEventSet.Object);

        // Use an empty list to simulate a non-existent event
        var events = Enumerable.Empty<Event>().BuildMock();
        _mockUnitOfWork.Setup(u => u.EventRepo.AsQueryable()).Returns(events);
        _mockUnitOfWork.Setup(u => u.EventRepo.AsQueryableNoTracking()).Returns(events);

        var eventToUpdate = new Event { Id = eventId };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _eventServices.UpdateEventAsync(eventId, eventToUpdate, userId));
    }

    [Fact]
    public async Task UpdateEventAsync_ShouldThrowAccessViolationException_ForUnauthorizedUser()
    {
        // Arrange
        var eventId = 1;
        var userId = 1;
        var lawyerId = 2; // Different from userId, implying the user does not own the event nor is the event's lawyer
        var existingEvent = new Event { Id = eventId, UserId = lawyerId, Lawyer = new Lawyer { UserId = lawyerId } };
        var events = new List<Event> { existingEvent }.BuildMock();

        _mockUnitOfWork.Setup(u => u.EventRepo.AsQueryable()).Returns(events);
        _mockUnitOfWork.Setup(u => u.EventRepo.AsQueryableNoTracking()).Returns(events);
        // _mockEventSet.Setup(m => m.Include(It.IsAny<Expression<Func<Event, object>>>())).Returns(_mockEventSet.Object);

        var eventToUpdate = new Event { Id = eventId };

        // Act & Assert
        await Assert.ThrowsAsync<AccessViolationException>(
            () => _eventServices.UpdateEventAsync(eventId, eventToUpdate, userId));
    }

    [Fact]
    public async Task UpdateEventAsync_ShouldThrowBadArgumentException_ForTimeCollision()
    {
        // Arrange
        var eventId = 1;
        var userId = 1;
        var existingEvent = new Event
        {
            Id = eventId,
            UserId = userId,
            Lawyer = new Lawyer { UserId = userId },
            StartTime = DateTime.Now.AddHours(1), // assuming future event
            EndTime = DateTime.Now.AddHours(2),
            Status = Status.ACCEPTED
        };
        var conflictEvent = new Event
        {
            Id = eventId+1,
            UserId = userId+1,
            Lawyer = new Lawyer { UserId = userId+1 },
            StartTime = DateTime.Now, // assuming future event
            EndTime = DateTime.Now,
            Status = Status.ACCEPTED
        };
        var events = new List<Event> { existingEvent, conflictEvent }.BuildMock();

        _mockUnitOfWork.Setup(u => u.EventRepo.AsQueryable()).Returns(events);
        _mockUnitOfWork.Setup(u => u.EventRepo.AsQueryableNoTracking()).Returns(events);
        // _mockEventSet.Setup(m => m.Include(It.IsAny<Expression<Func<Event, object>>>())).Returns(_mockEventSet.Object);

        var eventToUpdate = new Event
        {
            Id = eventId+1,
            StartTime = existingEvent.StartTime,
            EndTime = existingEvent.EndTime,
            Status = Status.ACCEPTED
        };

        // Mock HasCollision to return true, indicating a collision.
        // _eventService.GetType().GetMethod("HasCollision", BindingFlags.NonPublic BindingFlags.Instance)
        //     .Invoke(_eventService, new object[] { eventToUpdate.StartTime, eventToUpdate.EndTime, eventId })
        //     .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<BadArgumentException>(
            () => _eventServices.UpdateEventAsync(eventId+1, eventToUpdate, userId+1));
    }
}