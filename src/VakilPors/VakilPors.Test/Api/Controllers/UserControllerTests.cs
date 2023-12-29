using VakilPors.Api.Controllers;

namespace VakilPors.Test.Api.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Pagination.EntityFrameworkCore.Extensions;
using System.Security.Claims;
using System.Threading.Tasks;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Shared.Response;
using Xunit;

public class UserControllerTests
{
    private readonly Mock<IUserServices> _mockUserServices = new Mock<IUserServices>();
    private readonly Mock<IMapper> _mockMapper = new Mock<IMapper>();
    private readonly Mock<ILogger<UserController>> _mockLogger = new Mock<ILogger<UserController>>();
    private readonly UserController _controller;
    private const string PhoneNumber = "1234567890"; // Assuming phone number for all tests

    public UserControllerTests()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, PhoneNumber), // Assuming claim type Name holds phone number
            // ... additional claims as needed
        }, "mock"));

        var controllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };

        _controller = new UserController(_mockUserServices.Object, _mockMapper.Object, _mockLogger.Object)
        {
            ControllerContext = controllerContext
        };
    }

    private string GetPhoneNumber()
    {
        // Setup your method to extract the PhoneNumber from the context
        return PhoneNumber;
    }

    [Fact]
    public async Task GetAllPaged_ReturnsOkWithPagedUsers()
    {
        // Arrange
        var pagedParams = new PagedParams { PageNumber = 1, PageSize = 10 };
        var sortParams = new SortParams { Sort = "CreatedAt", IsAscending = true };
        string? query = null;
        int? roleId = null;

        var pagedUsers = new Pagination<UserDto>(new List<UserDto>(),10) // Assuming there is a Pagination DTO
        {
            // Populate with test data
        };

        _mockUserServices.Setup(service => service.GetUsers(query, roleId, pagedParams, sortParams))
            .ReturnsAsync(pagedUsers)
            .Verifiable();

        // Act
        var result = await _controller.GetAllPaged(query, roleId, pagedParams, sortParams);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var appResponse = Assert.IsType<AppResponse<Pagination<UserDto>>>(okResult.Value);
        Assert.Equal("success", appResponse.Message);
        Assert.Equal(pagedUsers, appResponse.Data);
        _mockUserServices.Verify();
    }

    // Additional test methods for verifying invalid input, different roles and their permissions, etc.
}