using Pagination.EntityFrameworkCore.Extensions;
using VakilPors.Api.Controllers;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Shared.Response;

namespace VakilPors.Test.Api.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Domain.Entities;
using Xunit;

public class ThreadControllerTests
{
    private readonly Mock<IThreadService> _mockThreadService = new Mock<IThreadService>();
    private readonly Mock<ILogger<ThreadController>> _mockLogger = new Mock<ILogger<ThreadController>>();
    private readonly Mock<IMapper> _mockMapper = new Mock<IMapper>();
    private readonly ThreadController _controller;
    private const int UserId = 1; // Assuming user ID is 1 for all tests

    public ThreadControllerTests()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, UserId.ToString()),
            new Claim("uid", UserId.ToString()),
        }, "mock"));

        var controllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };

        _controller = new ThreadController(_mockThreadService.Object, _mockLogger.Object)
        {
            ControllerContext = controllerContext
        };
    }

    private int GetUserId()
    {
        // Setup your method to extract the UserID from the context
        return UserId;
    }

    [Fact]
    public async Task CreateThread_ReturnsOkWithSuccessMessage()
    {
        // Arrange
        var threadDto = new ThreadDto { /* ... properties setup ... */ };
        var result = new ThreadDto(){ /* ... properties setup ... */ };
        _mockThreadService.Setup(service => service.CreateThread(GetUserId(), threadDto))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.CreateThread(threadDto);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
        var appResponse = Assert.IsType<AppResponse<object>>(okObjectResult.Value);
        Assert.Equal("thread created", appResponse.Message);
    }

    [Fact]
    public async Task UpdateThread_ReturnsOkWithSuccessMessage()
    {
        // Arrange
        var threadDto = new ThreadDto { /* ... properties setup ... */ };
        var result = new ThreadDto{ /* ... properties setup ... */ };
        _mockThreadService.Setup(service => service.UpdateThread(GetUserId(), threadDto))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.UpdateThread(threadDto);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
        var appResponse = Assert.IsType<AppResponse<object>>(okObjectResult.Value);
        Assert.Equal("thread updated", appResponse.Message);
    }

    [Fact]
    public async Task DeleteThread_ReturnsOkWithSuccessMessage()
    {
        // Arrange
        int threadId = 1;
        var result = true;
        _mockThreadService.Setup(service => service.DeleteThread(GetUserId(), threadId))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.DeleteThread(threadId);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
        var appResponse = Assert.IsType<AppResponse<object>>(okObjectResult.Value);
        Assert.Equal("thread deleted", appResponse.Message);
    }
// Continuing from the previous example...

[Fact]
public async Task GetThreadList_ReturnsOkWithThreadList()
{
    // Arrange
    var threadList = new List<ThreadDto> { /* ... Populate with test data ... */ };
    _mockThreadService.Setup(service => service.GetThreadList(GetUserId()))
        .ReturnsAsync(threadList);

    // Act
    var actionResult = await _controller.GetThreadList();

    // Assert
    var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
    var appResponse = Assert.IsType<AppResponse<object>>(okObjectResult.Value);
    Assert.Equal("success", appResponse.Message);
    Assert.Equal(threadList, appResponse.Data);
}

[Fact]
public async Task GetThreadWithComments_ReturnsOkWithThreadAndComments()
{
    // Arrange
    int threadId = 1;
    var pagedParams = new PagedParams { /* ... Populate with test data, e.g., page number, size ... */ };
    var threadWithComments = new ThreadWithCommentsDto(){ /* ... Populate with test data ... */ };
    _mockThreadService.Setup(service => service.GetThreadWithComments(GetUserId(), threadId, pagedParams))
        .ReturnsAsync(threadWithComments);

    // Act
    var actionResult = await _controller.GetThreadWithComments(threadId, pagedParams);

    // Assert
    var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
    var appResponse = Assert.IsType<AppResponse<object>>(okObjectResult.Value);
    Assert.Equal("success", appResponse.Message);
    Assert.Equal(threadWithComments, appResponse.Data);
}

[Fact]
public async Task LikeThread_ReturnsOkWithSuccessMessage()
{
    // Arrange
    int threadId = 1;
    var result = 2; // Assuming the like action returns a boolean
    _mockThreadService.Setup(service => service.LikeThread(GetUserId(), threadId))
        .ReturnsAsync(result);

    // Act
    var actionResult = await _controller.LikeThread(threadId);

    // Assert
    var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
    var appResponse = Assert.IsType<AppResponse<object>>(okObjectResult.Value);
    Assert.Equal("success", appResponse.Message);
    Assert.Equal(2,appResponse.Data);
}

[Fact]
public async Task UndoLikeThread_ReturnsOkWithSuccessMessage()
{
    // Arrange
    int threadId = 1;
    var result = 2; // Assuming the unlike action returns a boolean
    _mockThreadService.Setup(service => service.UndoLikeThread(GetUserId(), threadId))
        .ReturnsAsync(result);

    // Act
    var actionResult = await _controller.UndoLikeThread(threadId);

    // Assert
    var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
    var appResponse = Assert.IsType<AppResponse<object>>(okObjectResult.Value);
    Assert.Equal("success", appResponse.Message);
    Assert.Equal(2,appResponse.Data);
}

[Fact]
public async Task SearchThread_ReturnsOkWithFilteredThreads()
{
    // Arrange
    var pagedParams = new PagedParams { /* ... Populate with test data ... */ };
    var sortParams = new SortParams { /* ... Populate with test data ... */ };
    string title = "Sample Thread";
    var filteredThreads = new Pagination<ForumThread>(new List<ForumThread>(),10) { /* ... Populate with test data ... */ };
    _mockThreadService.Setup(service => service.SearchThread(title, pagedParams, sortParams, GetUserId()))
        .ReturnsAsync(filteredThreads);

    // Act
    var actionResult = await _controller.SearchThread(pagedParams, sortParams, title);

    // Assert
    var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
    var appResponse = Assert.IsType<AppResponse<object>>(okObjectResult.Value);
    Assert.Equal("success", appResponse.Message);
    Assert.Equal(filteredThreads, appResponse.Data);
}

// Additional test methods for any other actions in the controller...
}
