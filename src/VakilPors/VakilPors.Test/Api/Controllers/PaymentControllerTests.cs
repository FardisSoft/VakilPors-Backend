using VakilPors.Api.Controllers;
using ZarinSharp.OutputTypes;

namespace VakilPors.Test.Api.Controllers;

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Moq;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Payment;
using Xunit;

public class PaymentControllerTests
{
    private readonly Mock<IPaymentServices> _mockPaymentServices = new Mock<IPaymentServices>();
    private readonly PaymentController _controller;
    private readonly Mock<HttpContext> _mockHttpContext = new Mock<HttpContext>();
    private readonly Mock<HttpRequest> _mockHttpRequest = new Mock<HttpRequest>();
    private readonly Mock<HttpResponse> _mockHttpResponse = new Mock<HttpResponse>();
    private readonly Mock<IUrlHelper> _mockUrlHelper = new Mock<IUrlHelper>();

    public PaymentControllerTests()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "TestPhoneNumber"),
            new Claim("uid", "1"),
        }, "TestAuthentication"));

        _mockHttpContext.Setup(x => x.User).Returns(user);
        _mockHttpContext.SetupGet(x => x.Request).Returns(_mockHttpRequest.Object);
        _mockHttpContext.SetupGet(x => x.Response).Returns(_mockHttpResponse.Object);

        _mockHttpRequest.SetupGet(r => r.Scheme).Returns("http");
        _mockHttpRequest.SetupGet(r => r.Host).Returns(HostString.FromUriComponent("http://localhost"));

        _controller = new PaymentController(_mockPaymentServices.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object },
            Url = _mockUrlHelper.Object
        };
    }

    [Fact]
    public async Task RequestPayment_InvokesPaymentServiceWithCorrectParametersAndReturnsSuccess()
    {
        // Arrange
        var requestPaymentDto = new RequestPaymentDto { Amount = 100, Description = "Test Payment" };
        var paymentResult = new RequestPaymentOutput { /* Fill with expected properties */ };
        _mockPaymentServices.Setup(s =>
            s.RequestPayment(It.IsAny<int>(), requestPaymentDto.Amount, requestPaymentDto.Description, It.IsAny<string>()))
            .ReturnsAsync(paymentResult);

        _mockUrlHelper.Setup(x =>
            x.Action(It.IsAny<UrlActionContext>()))
            .Returns("/payment/verify");

        // Act
        var actionResult = await _controller.RequestPayment(requestPaymentDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(paymentResult, okResult.Value);
        // _mockPaymentServices.Verify(mock =>
        //     mock.RequestPayment(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task VerifyPayment_RedirectsToCorrectUrlWithQueryString()
    {
        // Arrange
        var authority = "TestAuthority";
        var status = "OK";
        var verificationResult = new VerifyPaymentOutput { /* Populate with test data */ };
        _mockPaymentServices.Setup(s =>
            s.VerifyPayment(authority, status))
            .ReturnsAsync(verificationResult);

        // Act
        var actionResult = await _controller.VerifyPayment(authority, status);

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(actionResult);
        Assert.Contains("/payment/verify", redirectResult.Url); // Additional assertions can be made based on exact expected URL format
    }
}