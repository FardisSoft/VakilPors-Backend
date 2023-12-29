using VakilPors.Api.Controllers;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.File;

namespace VakilPors.Test.Api.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.IO;
using System.Threading.Tasks;
using Xunit;

public class FileControllerTests
{
    private readonly Mock<IAwsFileService> _mockFileService = new Mock<IAwsFileService>();
    private readonly FileController _controller;

    public FileControllerTests()
    {
        _controller = new FileController(_mockFileService.Object);
    }

    private IFormFile SetupMockFile(string fileName)
    {
        var fileMock = new Mock<IFormFile>();
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write("dummy file content");
        writer.Flush();
        ms.Position = 0;

        fileMock.Setup(_ => _.FileName).Returns(fileName);
        fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
        fileMock.Setup(_ => _.Length).Returns(ms.Length);
        
        return fileMock.Object;
    }

    [Fact]
    public async Task Upload_ShouldReturnFileUrl()
    {
        var file = SetupMockFile("test.txt");
        var expectedUrl = "https://example.com/test.txt";

        _mockFileService.Setup(s => s.UploadAsync(file)).ReturnsAsync(expectedUrl);

        var result = await _controller.Upload(file);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedUrl, okResult.Value);
    }

    [Fact]
    public async Task UploadFileMessage_ShouldReturnMessage()
    {
        var file = SetupMockFile("test.txt");
        var expectedMessage = new FileDto();

        _mockFileService.Setup(s => s.UploadFileMessageAsync(file)).ReturnsAsync(expectedMessage);

        var result = await _controller.UploadFileMessage(file);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedMessage, okResult.Value);
    }

    [Fact]
    public void GetUrl_ShouldReturnFileUrl()
    {
        var fileKey = "test.txt";
        var expectedUrl = "https://example.com/test.txt";

        _mockFileService.Setup(s => s.GetFileUrl(fileKey)).Returns(expectedUrl);

        var result = _controller.GetUrl(fileKey);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedUrl, okResult.Value);
    }

    [Fact]
    public async Task Download_ShouldReturnFileStream()
    {
        var fileKey = "test.txt";
        var expectedStream = new MemoryStream();

        _mockFileService.Setup(s => s.DownloadAsync(fileKey)).ReturnsAsync(expectedStream);

        var result = await _controller.Download(fileKey);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedStream, okResult.Value);
    }
}
