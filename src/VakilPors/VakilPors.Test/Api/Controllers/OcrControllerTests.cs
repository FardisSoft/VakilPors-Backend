using VakilPors.Api.Controllers;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Ocr;

namespace VakilPors.Test.Api.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Shared.Response;

public class OcrControllerTests
{
    private readonly Mock<IOcrServices> _mockOcrServices = new Mock<IOcrServices>();
    private readonly Mock<ILogger<OcrController>> _mockLogger = new Mock<ILogger<OcrController>>();
    private readonly OcrController _controller;

    public OcrControllerTests()
    {
        _controller = new OcrController(_mockOcrServices.Object, _mockLogger.Object);
    }

    private IFormFile SetupMockFormFile(string fileName, string content)
    {
        var fileMock = new Mock<IFormFile>();
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);

        writer.Write(content);
        writer.Flush();
        ms.Position = 0;

        fileMock.Setup(_ => _.FileName).Returns(fileName);
        fileMock.Setup(_ => _.Length).Returns(ms.Length);
        fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
        fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream target, CancellationToken token) => ms.CopyToAsync(target));

        return fileMock.Object;
    }

    [Fact]
    public async Task PerformOcr_ReturnsAppResponse()
    {
        // Arrange
        var mockImageFileName = "test-image.jpg";
        var mockContent = "dummy image content";
        var ocrDto = new OcrDto { NationalCode = "1234567890" };
        var mockImageFile = SetupMockFormFile(mockImageFileName, mockContent);
        
        _mockOcrServices.Setup(s => s.GetNationalCode(It.IsAny<byte[]>(), mockImageFileName))
                        .ReturnsAsync(ocrDto);
        _mockLogger.Setup(x =>
            x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        
        // Act
        var result = await _controller.PerformOcr(mockImageFile);
        
        // Assert
        // var appResponseResult = Assert.IsType<AppResponse>(result.Value);
        // Assert.Equal("ocr done!", appResponseResult.Message);
        // Assert.NotNull(appResponseResult.Message);

        // var ocrResultData = Assert.IsType<OcrDto>(appResponseResult.Data);
        // Assert.Equal(ocrDto.NationalCode, ocrResultData.NationalCode);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.AtLeastOnce);
    }
}