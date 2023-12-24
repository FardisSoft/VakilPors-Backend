using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using VakilPors.Api.Controllers;
using VakilPors.Core.Exceptions;

namespace VakilPors.Test.Api.Controllers;

using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.User;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VakilPors.Shared.Response;

public class CustomerControllerTests
{
    private readonly Mock<IUserServices> _mockUserServices = new Mock<IUserServices>();
    private readonly Mock<ILogger<CustomerController>> _mockLogger = new Mock<ILogger<CustomerController>>();
    private readonly CustomerController _controller;

    public CustomerControllerTests()
    {
        _controller = new CustomerController(_mockUserServices.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnUpdatedUser()
    {
        // Arrange
        var userDto = new UserDto { UserName = "test_user" };
        _mockUserServices.Setup(service => service.UpdateUser(It.IsAny<UserDto>()))
                         .ReturnsAsync(userDto);

        // Act
        var result = await _controller.UpdateUser(userDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<AppResponse<UserDto>>(okResult.Value);
        Assert.Equal("Profile Updated", response.Message);
        Assert.Equal(userDto, response.Data);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<UserDto> { new UserDto(), new UserDto() };
        _mockUserServices.Setup(service => service.GetAllUsers())
                         .ReturnsAsync(users);

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<AppResponse<object>>(okResult.Value);
        Assert.Equal("success", response.Message);
        Assert.Equal(users, response.Data);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUser()
    {
        // Arrange
        int userId = 1;
        var user = new UserDto { Id = userId };
        _mockUserServices.Setup(service => service.GetUserById(userId))
                         .ReturnsAsync(user);

        // Act
        var result = await _controller.GetUserById(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<AppResponse<object>>(okResult.Value);
        Assert.Equal("success", response.Message);
        Assert.Equal(user, response.Data);
    }


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
    
    [Fact]
    public async Task GetCurrentUser_ShouldReturnCurrentUser()
    {
        // Arrange
        string userId = "1";
        SetUserContext(userId);

        var userDto = new UserDto { Id = Convert.ToInt32(userId) };
        _mockUserServices.Setup(service => service.GetUserById(Convert.ToInt32(userId)))
                         .ReturnsAsync(userDto);

        // Act
        var result = await _controller.GetCurrentUser();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<AppResponse<object>>(okResult.Value);
        Assert.Equal("success", response.Message);
        Assert.Equal(userDto, response.Data);
    }

}