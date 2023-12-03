using System.Net;
using Moq;
using Moq.Protected;
using VakilPors.Core.Domain.Dtos.Ocr;
using VakilPors.Core.Services;
using System.Net.Http.Json;

namespace VakilPors.Test.Core.Services;
public class OcrServicesTests
{
    private const string DjangoApiUrl = "https://fardissoft.pythonanywhere.com/ocr/Image/";
    [Fact]
    public async Task GetNationalCode_ReturnsOcrDto_WhenRequestIsSuccessful()
    {
        // Arrange
        var expectedOcrDto = new OcrDto { NationalCode = "1234567890" };
        var fakeHttpMessageHandler = new Mock<HttpMessageHandler>();
        fakeHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                nameof(HttpClient.SendAsync),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri.ToString().Equals(DjangoApiUrl)),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(expectedOcrDto)
            });

        var client = new HttpClient(fakeHttpMessageHandler.Object);
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

        var service = new OcrServices(httpClientFactoryMock.Object);

        // Act
        var result = await service.GetNationalCode(new byte[] {}, "test.jpg");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedOcrDto.NationalCode, result.NationalCode);
    }

    [Fact]
    public async Task GetNationalCode_ThrowsBadImageFormatException_WhenBadRequestStatusCode()
    {
        // Arrange
        var fakeHttpMessageHandler = new Mock<HttpMessageHandler>();
        fakeHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                nameof(HttpClient.SendAsync),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
            });

        var client = new HttpClient(fakeHttpMessageHandler.Object);
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

        var service = new OcrServices(httpClientFactoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<BadImageFormatException>(() => service.GetNationalCode(new byte[] {}, "test.jpg"));
    }

}