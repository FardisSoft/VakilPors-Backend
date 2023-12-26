using Microsoft.AspNetCore.Http;
using VakilPors.Api.Controllers;

namespace VakilPors.Test.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Report;
using VakilPors.Shared.Response;
using Xunit;

public class ReportControllerTests
{
    private readonly Mock<IReportServices> _mockReportService = new Mock<IReportServices>();
    private readonly ReportController _controller;

    public ReportControllerTests()
    {
        _controller = new ReportController(_mockReportService.Object);
        // ... Configure the controller context if needed (for example, User information)
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkResultWithReports()
    {
        // Arrange
        var reports = new List<ReportDto>(){ /* properties of an anonymous object matching the return of GetAllReport */ };
        _mockReportService.Setup(service => service.GetAllReport()).ReturnsAsync(reports);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var appResponse = Assert.IsType<AppResponse<object>>(okResult.Value);
        Assert.Equal("success", appResponse.Message);
        Assert.Equal(reports, appResponse.Data);
    }

    [Fact]
    public async Task PostReport_ShouldReturnOkResult()
    {
        // Arrange
        var postReportDto = new PostReportDto { /* set properties as needed for testing */ };
        var serviceResponse = true;
        _mockReportService.Setup(service => service.PostReport(postReportDto)).ReturnsAsync(serviceResponse);

        // Act
        var result = await _controller.PostReport(postReportDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var appResponse = Assert.IsType<AppResponse<object>>(okResult.Value);
        Assert.Equal("success", appResponse.Message);
        Assert.Equal(serviceResponse, appResponse.Data);
    }

    [Fact]
    public async Task DeleteReport_WhenSuccessful_ShouldReturnNoContentResult()
    {
        // Arrange
        int reportId = 1;
        _mockReportService.Setup(service => service.DeleteReport(reportId)).ReturnsAsync(true);
        
        // Act
        var result = await _controller.DeleteReport(reportId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteReport_WhenNotFound_ShouldReturnNotFoundResult()
    {
        // Arrange
        int reportId = 1;
        _mockReportService.Setup(service => service.DeleteReport(reportId)).ReturnsAsync(false);
        
        // Act
        var result = await _controller.DeleteReport(reportId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteReport_WhenExceptionThrown_ShouldReturnInternalServerError()
    {
        // Arrange
        int reportId = 1;
        _mockReportService.Setup(service => service.DeleteReport(reportId)).ThrowsAsync(new Exception());
        
        // Act
        var result = await _controller.DeleteReport(reportId);

        // Assert
        var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
    }
}