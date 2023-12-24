using Pagination.EntityFrameworkCore.Extensions;
using VakilPors.Api.Controllers;

namespace VakilPors.Test.Api.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Shared.Response;
using Xunit;

public class ThreadCommentControllerTests
{
    private readonly Mock<IThreadCommentService> _mockThreadCommentService = new Mock<IThreadCommentService>();
    private readonly Mock<ILogger<ThreadCommentController>> _mockLogger = new Mock<ILogger<ThreadCommentController>>();
    private readonly ThreadCommentController _controller;
    private readonly int _userId = 1;

    public ThreadCommentControllerTests()
    {
        _controller = new ThreadCommentController(_mockLogger.Object, _mockThreadCommentService.Object);
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = CreateClaimsPrincipal(_userId) }
        };
    }

    // Helper method to create a ClaimsPrincipal for HttpContext
    private static ClaimsPrincipal CreateClaimsPrincipal(int userId)
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()),new Claim("uid", userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        return principal;
    }

    [Fact]
    public async Task CreateComment_ShouldReturnOkResult()
    {
        // Arrange
        var commentDto = new ThreadCommentDto { /* ... Properties setup ... */ };
        var serviceResult = new ThreadCommentDto(){ /* ... Properties setup ... */ };
        _mockThreadCommentService.Setup(s => s.CreateComment(_userId, commentDto))
            .ReturnsAsync(serviceResult);
        
        // Act
        var result = await _controller.CreateComment(commentDto);
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var appResponse = Assert.IsType<AppResponse<object>>(okResult.Value);
        Assert.NotNull(appResponse);
        Assert.Equal("comment created", appResponse.Message);
        Assert.Equal(serviceResult, appResponse.Data);
    }

    // Similar tests should be written for each of the other action methods in the controller.
    // They would follow the same pattern of mocking the service method, invoking the controller action,
    // and then asserting the expected type of result is returned with the correct values.

    // ...

// Continuing from ThreadCommentControllerTests

[Fact]
public async Task UpdateComment_ShouldReturnOkResult()
{
    // Arrange
    var commentDto = new ThreadCommentDto { /* ... Properties setup ... */ };
    _mockThreadCommentService.Setup(s => s.UpdateComment(_userId, commentDto))
        .ReturnsAsync(commentDto); // assuming the service returns the updated DTO
    
    // Act
    var result = await _controller.UpdateComment(commentDto);
    
    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var appResponse = Assert.IsType<AppResponse<object>>(okResult.Value);
    Assert.NotNull(appResponse);
    Assert.Equal("comment updated", appResponse.Message);
    Assert.Equal(commentDto, appResponse.Data);
}

[Fact]
public async Task DeleteComment_ShouldReturnOkResult()
{
    // Arrange
    int commentId = 1;
    _mockThreadCommentService.Setup(s => s.DeleteComment(_userId, commentId))
        .ReturnsAsync(true); // assuming the service returns true for successfully deleted
    
    // Act
    var result = await _controller.DeleteComment(commentId);
    
    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var appResponse = Assert.IsType<AppResponse<object>>(okResult.Value);
    Assert.NotNull(appResponse);
    Assert.Equal("comment deleted", appResponse.Message);
    Assert.True((bool)appResponse.Data);
}

[Fact]
public async Task GetCommentsForThread_ShouldReturnOkResultWithComments()
{
    // Arrange
    var pagedParams = new PagedParams { /* ... Properties setup ... */ };
    int threadId = 1;
    var comments = new Pagination<ThreadCommentDto>(new List<ThreadCommentDto>(),10){ /* ... Properties setup ... */ };
    _mockThreadCommentService.Setup(s => s.GetCommentsForThread(_userId, threadId, pagedParams))
        .ReturnsAsync(comments); // assuming the service returns a collection of comments
    
    // Act
    var result = await _controller.GetCommentsForThread(threadId, pagedParams);
    
    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var appResponse = Assert.IsType<AppResponse<object>>(okResult.Value);
    Assert.NotNull(appResponse);
    Assert.Equal("success", appResponse.Message);
    Assert.Equal(comments, appResponse.Data);
}

[Fact]
public async Task GetCommentById_ShouldReturnOkResultWithComment()
{
    // Arrange
    int commentId = 1;
    var comment = new ThreadCommentDto() { /* ... Properties setup ... */ };
    _mockThreadCommentService.Setup(s => s.GetCommentById(_userId, commentId))
        .ReturnsAsync(comment); // assuming the service returns a single comment DTO
    
    // Act
    var result = await _controller.GetCommentById(commentId);
    
    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var appResponse = Assert.IsType<AppResponse<object>>(okResult.Value);
    Assert.NotNull(appResponse);
    Assert.Equal("success", appResponse.Message);
    Assert.Equal(comment, appResponse.Data);
}

[Fact]
public async Task LikeComment_ShouldReturnOkResult()
{
    // Arrange
    int commentId = 1;
    _mockThreadCommentService.Setup(s => s.LikeComment(_userId, commentId))
        .ReturnsAsync(1); // assuming the service returns true for successfully liked
    
    // Act
    var result = await _controller.LikeComment(commentId);
    
    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var appResponse = Assert.IsType<AppResponse<object>>(okResult.Value);
    Assert.NotNull(appResponse);
    Assert.Equal("success", appResponse.Message);
    Assert.Equal(1,appResponse.Data);
}

[Fact]
public async Task UndoLikeComment_ShouldReturnOkResult()
{
    // Arrange
    int commentId = 1;
    _mockThreadCommentService.Setup(s => s.UndoLikeComment(_userId, commentId))
        .ReturnsAsync(1); // assuming the service returns true for successfully undoing a like
    
    // Act
    var result = await _controller.UndoLikeComment(commentId);
    
    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var appResponse = Assert.IsType<AppResponse<object>>(okResult.Value);
    Assert.NotNull(appResponse);
    Assert.Equal("success", appResponse.Message);
    Assert.Equal(1,appResponse.Data);
}

[Fact]
public async Task SetAsAnswer_ShouldReturnOkResult()
{
    // Arrange
    int commentId = 1;
    _mockThreadCommentService.Setup(s => s.SetAsAnswer(_userId, commentId))
        .ReturnsAsync(true); // assuming the service returns true for successfully setting as an answer
    
    // Act
    var result = await _controller.SetAsAnswer(commentId);
    
    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var appResponse = Assert.IsType<AppResponse<object>>(okResult.Value);
    Assert.NotNull(appResponse);
    Assert.Equal("success", appResponse.Message);
    Assert.True((bool)appResponse.Data);
}

[Fact]
public async Task UndoSetAsAnswer_ShouldReturnOkResult()
{
    // Arrange
    int commentId = 1;
    _mockThreadCommentService.Setup(s => s.UndoSetAsAnswer(_userId, commentId))
        .ReturnsAsync(true); // assuming the service returns true for successfully undoing set as answer
    
    // Act
    var result = await _controller.UndoSetAsAnswer(commentId);
    
    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var appResponse = Assert.IsType<AppResponse<object>>(okResult.Value);
    Assert.NotNull(appResponse);
    Assert.Equal("success", appResponse.Message);
    Assert.True((bool)appResponse.Data);
}

// ... Other test cases ...
}