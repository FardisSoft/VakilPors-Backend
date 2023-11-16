using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using VakilPors.Core.Exceptions;
using VakilPors.Core.Services;

namespace VakilPors.Test.Core.Services;

public class SmsSenderTests
{
    [Fact]
    public async Task SendSmsAsync_Failure_Catch()
    {
        // Arrange
        var configuration = new Mock<IConfiguration>();
        var logger = new Mock<ILogger<SMSSender>>();
        var httpClientFactory = new Mock<IHttpClientFactory>();
        var httpClient = new HttpClient();
    
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
    
        var smsSender = new SMSSender(configuration.Object, logger.Object, httpClientFactory.Object);
    
        // Act
        await smsSender.SendSmsAsync("123456789", "Test message");
    }

    [Fact]
    public async Task SendSmsAsync_Successful()
    {
        // Arrange
        var configuration = new Mock<IConfiguration>();
        var logger = new Mock<ILogger<SMSSender>>();
        var httpClientFactory = new Mock<IHttpClientFactory>();
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK,Content = new StringContent("200")});

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
        
        
        var smsSender = new SMSSender(configuration.Object, logger.Object, httpClientFactory.Object);
        var result =await smsSender.SendSmsAsync("123456789", "Test message");
        // Act and Assert
        Assert.Equal(200,result);
    }
    [Fact]
    public async Task SendSmsAsync_Failure()
    {
        // Arrange
        var configuration = new Mock<IConfiguration>();
        var logger = new Mock<ILogger<SMSSender>>();
        var httpClientFactory = new Mock<IHttpClientFactory>();
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
        
        
        var smsSender = new SMSSender(configuration.Object, logger.Object, httpClientFactory.Object);
        // Act and Assert
        await Assert.ThrowsAsync<InternalServerException>(() => smsSender.SendSmsAsync("123456789", "Test message"));
    }
}
