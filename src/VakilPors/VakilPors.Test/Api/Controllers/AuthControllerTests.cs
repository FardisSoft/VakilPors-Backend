using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Moq;
using VakilPors.Api.Controllers;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Shared.Response;

namespace VakilPors.Test.Api.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthServices> _authManagerMock;
    private readonly Mock<ILogger<AuthController>> _loggerMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authManagerMock = new Mock<IAuthServices>();
        _loggerMock = new Mock<ILogger<AuthController>>();
        _controller = new AuthController(_authManagerMock.Object, _loggerMock.Object);
    }
    // Using Xunit and Moq frameworks here for demonstration

    [Fact]
    public async Task Register_ValidCall()
    {
        // Arrange
        SignUpDto signUpDto = new SignUpDto
        {
            // Assume we fill the necessary properties
            PhoneNumber = "1234567890"
        };

        _authManagerMock.Setup(x => x.Register(signUpDto))
            .ReturnsAsync(new List<IdentityError>());

        // Act
        var result = await _controller.Register(signUpDto);
    
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        // Additional assertions to check the contents of the response
    }

    [Fact]
    public async Task Login_ValidCall()
    {
        // Arrange
        LoginDto loginDto = new LoginDto
        {
            // Assume we fill the necessary properties
            PhoneNumber = "1234567890"
        };

        _authManagerMock.Setup(x => x.Login(loginDto))
            .ReturnsAsync(new LoginResponseDto
            {
                // Filled with expected return values
            });

        // Act
        var result = await _controller.Login(loginDto);
    
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        // Additional assertions to check the contents of the response
    }
    [Fact]
    public async Task RefreshToken_ValidCall()
    {
        // Arrange
        LoginResponseDto loginResponseDto = new LoginResponseDto
        {
            // Assume we fill the necessary properties
        };

        _authManagerMock.Setup(x => x.VerifyRefreshToken(loginResponseDto))
            .ReturnsAsync(new LoginResponseDto
            {
                // Filled with expected return values
            });

        // Act
        var result = await _controller.RefreshToken(loginResponseDto);
    
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        // Additional assertions to check the contents of the response
    }
    [Fact]
    public async Task ForgetPassword_ValidCall()
    {
        // Arrange
        ForgetPasswordDto forgetPasswordDto = new ForgetPasswordDto
        {
            PhoneNumber = "1234567890"
            // Assume the rest of necessary properties are filled
        };

        _authManagerMock.Setup(x => x.CreateAndSendForgetPasswordToken(forgetPasswordDto))
            .Returns(Task.CompletedTask); // Assuming void return

        // Act
        var result = await _controller.ForgetPassword(forgetPasswordDto);
    
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        // Additional assertions to check the contents of the response
    }
    [Fact]
    public async Task ResetPassword_ValidCall()
    {
        // Arrange
        ResetPasswordDto resetPasswordDto = new ResetPasswordDto
        {
            PhoneNumber = "1234567890",
            // Assume the rest of necessary properties are filled
        };

        _authManagerMock.Setup(x => x.ResetPassword(resetPasswordDto))
            .Returns(Task.CompletedTask); // Assuming void return

        // Act
        var result = await _controller.ResetPassword(resetPasswordDto);
    
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        // Additional assertions to check the contents of the response
    }
    [Fact]
    public async Task ActivateAccount_ValidCall()
    {
        // Arrange
        ActivateAccountDto activateAccountDto = new ActivateAccountDto
        {
            PhoneNumber = "1234567890",
            // Assume the rest of necessary properties are filled
        };

        _authManagerMock.Setup(x => x.ActivateAccount(activateAccountDto))
            .Returns(Task.CompletedTask); // Assuming void return

        // Act
        var result = await _controller.ActivateAccount(activateAccountDto);
    
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        // Additional assertions to check the contents of the response
    }
    [Fact]
    public async Task SendActivationCode_ValidCall()
    {
        // Arrange
        string phoneNumber = "1234567890";

        _authManagerMock.Setup(x => x.SendActivationCode(phoneNumber))
            .Returns(Task.CompletedTask); // Assuming void return

        // Act
        var result = await _controller.SendActivationCode(phoneNumber);
    
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        // Additional assertions to check the contents of the response
    }
    [Fact]
    public async Task ResetPassword_InvalidModelState()
    {
        // Arrange
        ResetPasswordDto resetPasswordDto = new ResetPasswordDto
        {
            // Assuming the properties are not filled or filled with invalid data
        };

        // Adding a model state error
        _controller.ModelState.AddModelError("Error", "Sample model state error");

        // Act
        var result = await _controller.ResetPassword(resetPasswordDto);
    
        // Assert
        // Check that the result is a BadRequestObjectResult
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);

        // Optionally, verify the nature of the bad request; it should contain the model state errors
        var appResponse = Assert.IsAssignableFrom<AppResponse<ModelStateDictionary>>(badRequestResult.Value);
        Assert.False(appResponse.IsSuccess);
        Assert.Equal("Fields validations resulted in errors!", appResponse.Message);
        Assert.Equal(HttpStatusCode.BadRequest, appResponse.StatusCode);
        Assert.Equal(1, appResponse.Data.Count); // Assuming we only added one error
    }
    [Fact]
    public async Task ActivateAccount_InvalidModelState()
    {
        // Arrange
        ActivateAccountDto activateAccountDto = new ActivateAccountDto
        {
            // Leave out necessary properties or assign invalid data
        };

        // Simulating an invalid model state
        _controller.ModelState.AddModelError("PhoneNumber", "PhoneNumber is required.");

        // Act
        var result = await _controller.ActivateAccount(activateAccountDto);
    
        // Assert
        // The action should return a BadRequestObjectResult when the model state is invalid
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);

        // Furthermore, assert the contents of the result to ensure proper error messages are returned
        var appResponse = Assert.IsAssignableFrom<AppResponse<ModelStateDictionary>>(badRequestResult.Value);
        Assert.False(appResponse.IsSuccess);
        Assert.Equal("Fields validations resulted in errors!", appResponse.Message);
        Assert.Equal(HttpStatusCode.BadRequest, appResponse.StatusCode);
    
        // Specifically, check the ModelState error that we added
        Assert.Contains(appResponse.Data, x => x.Key == "PhoneNumber");
        var errors = appResponse.Data["PhoneNumber"].Errors;
        Assert.Equal(1, errors.Count);
        Assert.Equal("PhoneNumber is required.", errors[0].ErrorMessage);
    }
    [Fact]
    public async Task SendActivationCode_WhenModelIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        string invalidPhoneNumber = ""; // invalid input
        _controller.ModelState.AddModelError("phoneNumber", "The phoneNumber field is required."); // Simulating a model state error

        // Act
        var result = await _controller.SendActivationCode(invalidPhoneNumber);

        // Assert
        // Expecting a BadRequest response due to invalid model state
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }
        [Fact]
        public async Task Register_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var signUpDto = new SignUpDto { PhoneNumber = "1234567890", Password = "Test@1234" };
        
            // Setting up the mock to simulate errors from the authentication process.
            var identityErrors = new List<IdentityError>
            {
                new IdentityError {Code = "code",Description = "Password too weak" }
            };
            _authManagerMock.Setup(x => x.Register(signUpDto)).ReturnsAsync(identityErrors);

            // Instantiating the controller and setting up the ModelState
        
            // Act
            var result = await _controller.Register(signUpDto);
        
            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var appResponse = Assert.IsType<AppResponse<ModelStateDictionary>>(badRequestResult.Value);
            Assert.Single(appResponse.Data);
            Assert.Equal("Password too weak", appResponse.Data.FirstOrDefault().Value.Errors[0].ErrorMessage);
        }
        
    
}