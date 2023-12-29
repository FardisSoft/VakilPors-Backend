using Pagination.EntityFrameworkCore.Extensions;
using VakilPors.Api.Controllers;

namespace VakilPors.Test.Api.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.Rate;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Response;
using Xunit;

public class RateControllerTests
{
    private readonly Mock<IRateService> _mockRateService = new Mock<IRateService>();
    private readonly Mock<ILogger<PremiumController>> _mockLogger = new Mock<ILogger<PremiumController>>();
    private readonly Mock<IMapper> _mockMapper = new Mock<IMapper>();
    private readonly RateController _controller;
    private readonly int _userId = 1;
    private readonly int _lawyerId = 2;

    public RateControllerTests()
    {
        _controller = new RateController(_mockRateService.Object, _mockLogger.Object, _mockMapper.Object);
        SetUserContext(_userId.ToString()); // This method should mock the User context in your base class
    }
    
    // Example of a test for GetRateStatus method.
    [Fact]
    public async Task GetRateStatus_ReturnsNotFound_WhenRateNotExists()
    {
        // Arrange
        _mockRateService.Setup(service => service.GetRateAsync(_userId, _lawyerId))
            .ReturnsAsync((RateDto)null);

        // Act
        var result = await _controller.GetRateStatus(_lawyerId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    // Example of a test for GetRateStatus method.
    [Fact]
    public async Task GetRateStatus_ReturnsOk_WhenRateExists()
    {
        // Arrange
        var rate = new RateDto() { /* ... Properties setup ... */ };
        _mockRateService.Setup(service => service.GetRateAsync(_userId, _lawyerId))
            .ReturnsAsync(rate);

        // Act
        var result = await _controller.GetRateStatus(_lawyerId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(rate, okResult.Value);
    }

    // Example of a test for GetAverageStatus method.
    [Fact]
    public async Task GetAverageStatus_ReturnsCorrectAverage()
    {
        // Arrange
        double expectedAverage = 4.5;
        _mockRateService.Setup(service => service.CalculateRatingAsync(_lawyerId))
            .ReturnsAsync(expectedAverage);

        // Act
        double average = await _controller.GetAverageStatus(_lawyerId);

        // Assert
        Assert.Equal(expectedAverage, average);
    }
    
    // Example of a test for AddRate method.
    [Fact]
    public async Task AddRate_ReturnsOkResult()
    {
        // Arrange
        var rateDto = new RateDto { /* ... Properties setup ... */ };

        // Act
        var result = await _controller.AddRate(rateDto, _lawyerId);

        // Assert
        Assert.IsType<OkResult>(result);
        _mockRateService.Verify(service => service.AddRateAsync(rateDto, _userId, _lawyerId), Times.Once());
    }

    // Example of a test for GetAllRates method.
    [Fact]
    public async Task GetAllRates_ReturnsMappedPagination()
    {
        // Arrange
        var pagedParams = new PagedParams { /* ... Properties setup ... */ };
        var rates = new Pagination<Rate>(new List<Rate>(),0) { /* ... Properties setup ... */ };
        var rateUserDtos = new Pagination<RateUserDto>(new List<RateUserDto>(),0) { /* ... Properties setup ... */ };

        _mockRateService.Setup(service => service.GetRatesPagedAsync(_lawyerId, pagedParams))
            .ReturnsAsync(rates);
        _mockMapper.Setup(mapper => mapper.Map<Pagination<RateUserDto>>(rates))
            .Returns(rateUserDtos);

        // Act
        var result = await _controller.GetAllRates(_lawyerId, pagedParams);

        // Assert
        // Assert.Equal(rateUserDtos, result);
    }

    // Example of a test for UpdateRate method.
    [Fact]
    public async Task UpdateRate_ReturnsOkResult()
    {
        // Arrange
        var rateDto = new RateDto { /* ... Properties setup ... */ };

        // Act
        var result = await _controller.UpdateRate(rateDto, _lawyerId);

        // Assert
        Assert.IsType<OkResult>(result);
        _mockRateService.Verify(service => service.UpdateRateAsync(rateDto, _userId, _lawyerId), Times.Once());
    }

    // Add other necessary tests ...
    
    // Method to mock the User context based on your MyControllerBase implementation
    private void SetUserContext(string userId)
    {
        var claims = new List<Claim>
        {
            new Claim("uid", userId),
            new Claim(ClaimTypes.NameIdentifier,"")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }
}