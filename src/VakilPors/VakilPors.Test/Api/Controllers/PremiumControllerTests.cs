using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using VakilPors.Api.Controllers;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.Premium;

namespace VakilPors.Test.Api.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Pagination.EntityFrameworkCore.Extensions;
using System.Threading.Tasks;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Response;
using Xunit;

public class PremiumControllerTests
{
    private readonly Mock<IPremiumService> _mockPremiumService = new Mock<IPremiumService>();
    private readonly Mock<ILogger<PremiumController>> _mockLogger = new Mock<ILogger<PremiumController>>();
    private readonly Mock<IMapper> _mockMapper = new Mock<IMapper>();
    private readonly PremiumController _controller;
    private readonly int _userId = 1;

    public PremiumControllerTests()
    {
        _controller = new PremiumController(_mockPremiumService.Object, _mockLogger.Object, _mockMapper.Object);
        SetUserContext(_userId.ToString()); // Simulate base controller functionality for User context
    }

    [Fact]
    public async Task GetPremiumStatus_SubscriptionExists_ReturnsSuccessStatus()
    {
        // Arrange
        var subscribedDto = new SubscribedDto { /* ... Properties setup ... */ };
        _mockPremiumService.Setup(service => service.GetPremiumStatus(_userId))
            .ReturnsAsync(subscribedDto);

        // Act
        var result = await _controller.GetPremiumStatus();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var appResponse = Assert.IsType<AppResponse<object>>(okResult.Value);
        Assert.Equal("success", appResponse.Message);
        Assert.NotNull(appResponse.Data);
    }

    [Fact]
    public async Task GetPremiumStatus_NoSubscription_ReturnsNotSubscribedMessage()
    {
        // Arrange
        _mockPremiumService.Setup(service => service.GetPremiumStatus(_userId))
            .ReturnsAsync(value: null);

        // Act
        var result = await _controller.GetPremiumStatus();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var appResponse = Assert.IsType<AppResponse<object>>(okResult.Value);
        Assert.Equal("user is not subscribed", appResponse.Message);
        Assert.Null(appResponse.Data);
    }

    [Fact]
    public async Task ActivatePremium_ValidPlan_ReturnsSuccessMessage()
    {
        // Arrange
        string premiumPlan = "gold";
        _mockPremiumService.Setup(service => service.ActivatePremium(premiumPlan.ToLower(), _userId))
            .Returns(Task.FromResult(new Subscribed()));

        // Act
        var result = await _controller.ActivatePremium(premiumPlan);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var appResponse = Assert.IsType<AppResponse<object>>(okResult.Value);
        Assert.Equal("success", appResponse.Message);
        Assert.Equal(premiumPlan, appResponse.Data);
    }

    [Fact]
    public async Task DeactivePremium_SubscriptionActive_ReturnsNoContent()
    {
        // Arrange
        _mockPremiumService.Setup(service => service.DeactivatePremium(_userId))
            .Returns(Task.CompletedTask);

        // Act
        await _controller.DeactivePremium();

        // No direct response to assert since DeactivePremium does not return an ActionResult,
        // but you may verify that the method on the service was called:
        _mockPremiumService.Verify(service => service.DeactivatePremium(_userId), Times.Once());
    }

    [Fact]
    public async Task GetAllSunbcriptionStatus_ValidRequest_ReturnsPagedSubscriptions()
    {
        // Arrange
        var pagedParams = new PagedParams { /* ... Properties setup ... */ };
        var sortParams = new SortParams { /* ... Properties setup ... */ };
        var paginatedData = new Pagination<SubscribedDto>(new List<SubscribedDto>(){},10) { /* ... Properties setup ... */ };

        // Assume the service returns a Pagination of Subscribed 
        // and then mapper maps it to Pagination of SubscribedDto
        _mockPremiumService.Setup(service => service.GetAllSubscriptionStatus(pagedParams, sortParams))
            .ReturnsAsync(new Pagination<Subscribed>(new List<Subscribed>(),10) { /* ... Properties setup ... */ });

        _mockMapper.Setup(m => m.Map<Pagination<SubscribedDto>>(It.IsAny<Pagination<Subscribed>>()))
            .Returns(paginatedData);

        // Act
        var result = await _controller.GetAllSunbcriptionStatus(pagedParams, sortParams);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var appResponse = Assert.IsType<AppResponse<Pagination<SubscribedDto>>>(okResult.Value);
        Assert.Equal("success", appResponse.Message);
        // Assert.Equal(paginatedData, appResponse.Data);
    }

    [Fact]
    public async Task GetAllSubscribedLawyersStatus_ValidRequest_ReturnsPagedLawyers()
    {
        // Arrange
        var pagedParams = new PagedParams { /* ... Properties setup ... */ };
        var sortParams = new SortParams { /* ... Properties setup ... */ };
        var paginatedData = new Pagination<SubscribedDto>(new List<SubscribedDto>(),10) { /* ... Properties setup ... */ };

        _mockPremiumService.Setup(service => service.GetAllSubscribedLawyersStatus(pagedParams, sortParams))
            .ReturnsAsync(new Pagination<Subscribed>(new List<Subscribed>(),10) { /* ... Properties setup ... */ });

        _mockMapper.Setup(m => m.Map<Pagination<SubscribedDto>>(It.IsAny<Pagination<Subscribed>>()))
            .Returns(paginatedData);

        // Act
        var result = await _controller.GetAllSubscribedLawyersStatus(pagedParams, sortParams);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var appResponse = Assert.IsType<AppResponse<Pagination<SubscribedDto>>>(okResult.Value);
        Assert.Equal("success", appResponse.Message);
        // Assert.Equal(paginatedData, appResponse.Data);
    }

    // ... Additional tests as needed ...

    // Helper method to mimic setting the user context that would be provided by the MyControllerBase
    private void SetUserContext(string userId)
    {
        var claims = new List<Claim>
        {
            new Claim("uid", userId)
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }
}
