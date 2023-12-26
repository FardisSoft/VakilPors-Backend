using VakilPors.Api.Controllers;

namespace VakilPors.Test.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.User;
using Xunit;

public class TelegramControllerTests
{
    private readonly Mock<ITelegramService> _mockTelegramService = new Mock<ITelegramService>();
    private readonly TelegramController _controller;

    public TelegramControllerTests()
    {
        _controller = new TelegramController(_mockTelegramService.Object);
        // Configure here if there's anything in MyControllerBase you need to mock or setup
    }

    [Fact]
    public async Task SaveChatId_ShouldCallSaveChatIdServiceMethodOnce()
    {
        // Arrange
        var telegramDto = new TelegramDto { /* Set properties as needed for test */ };
        _mockTelegramService.Setup(service => service.SaveChatId(telegramDto))
            .Returns(Task.CompletedTask)
            .Verifiable("SaveChatId was not called on the ITelegramService");

        // Act
        await _controller.SaveChatId(telegramDto);

        // Assert
        _mockTelegramService.Verify();
    }
}