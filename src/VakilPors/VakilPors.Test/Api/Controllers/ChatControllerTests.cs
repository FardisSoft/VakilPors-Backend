using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using VakilPors.Api.Controllers;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Response;

namespace VakilPors.Test.Api.Controllers;

public class ChatControllerTests
{
    private readonly Mock<ILogger<ChatController>> _mockLogger;
    private readonly Mock<IChatServices> _mockChatServices;
    private readonly ChatController _controller;
    private readonly int _userId = 1;
    private readonly string _phoneNumber = "1234567890";
    private readonly int _chatId = 10;

    public ChatControllerTests()
    {
        _mockLogger = new Mock<ILogger<ChatController>>();
        _mockChatServices = new Mock<IChatServices>();

        _controller = new ChatController(_mockLogger.Object, _mockChatServices.Object);
        SetUserContext("1234567890", "1");
        // Mock User context if needed using, e.g. _controller.ControllerContext.HttpContext = ...
    }
    private void SetUserContext(string phoneNumber, string userId)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, phoneNumber),
            new Claim("uid", userId)
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }
    // This test method verifies the GetChats action.
    [Fact]
    public async Task GetChats_ReturnsChatsSuccessfully()
    {
        // Arrange
        var mockChats = new List<Chat> { /* ... populate test data ... */ };
        _mockChatServices.Setup(s => s.GetChatsOfUser(_userId)).ReturnsAsync(mockChats);

        // Act
        var result = await _controller.GetChats();

        // Assert
        var appResponse = Assert.IsType<AppResponse<ICollection<Chat>>>(result.Value);
        Assert.NotNull(appResponse);
        Assert.Equal(mockChats.Count, appResponse.Data.Count);
        _mockChatServices.Verify(s => s.GetChatsOfUser(_userId), Times.Once);
    }

    // This test method verifies the GetChatsWithMessages action.
    [Fact]
    public async Task GetChatsWithMessages_ReturnsChatsWithMessagesSuccessfully()
    {
        // Arrange
        var mockChatsWithMessages = new List<Chat> { /* ... populate test data ... */ };
        _mockChatServices.Setup(s => s.GetChatsWithMessagesOfUser(_userId)).ReturnsAsync(mockChatsWithMessages);

        // Act
        var result = await _controller.GetChatsWithMessages();

        // Assert
        var appResponse = Assert.IsType<AppResponse<ICollection<Chat>>>(result.Value);
        Assert.NotNull(appResponse);
        Assert.Equal(mockChatsWithMessages.Count, appResponse.Data.Count);
        _mockChatServices.Verify(s => s.GetChatsWithMessagesOfUser(_userId), Times.Once);
    }

    [Fact]
    public async Task GetChatMessages_ReturnsMessagesSuccessfully()
    {
        // Arrange
        var mockMessages = new List<ChatMessage> { /* ... populate test data ... */ };
        _mockChatServices.Setup(s => s.GetMessagesOfChat(_userId, _chatId)).ReturnsAsync(mockMessages);

        // Act
        var result = await _controller.GetChatMessages(_chatId);

        // Assert
        var appResponse = Assert.IsType<AppResponse<ICollection<ChatMessage>>>(result.Value);
        Assert.NotNull(appResponse);
        Assert.Equal(mockMessages.Count, appResponse.Data.Count);
        _mockChatServices.Verify(s => s.GetMessagesOfChat(_userId, _chatId), Times.Once);
    }

    [Fact]
    public async Task StartChat_CreatesOrFetchesChatSuccessfully()
    {
        // Arrange
        var receiverUserId = 2;
        var mockChat = new Chat { /* ... populate test data ... */ };
        _mockChatServices.Setup(s => s.CreateChat(receiverUserId, _userId)).ReturnsAsync(mockChat);

        // Act
        var result = await _controller.StartChat(receiverUserId);

        // Assert
        var appResponse = Assert.IsType<AppResponse<Chat>>(result.Value);
        Assert.NotNull(appResponse);
        Assert.Equal(mockChat, appResponse.Data);
        _mockChatServices.Verify(s => s.CreateChat(receiverUserId, _userId), Times.Once);
    }

    // Additional test cases to consider:
    // - Test cases for unauthorized access
    // - Test cases for null or default responses from the chat services
    // - Test cases for exceptional scenarios like services throwing exceptions
}