using Pagination.EntityFrameworkCore.Extensions;
using VakilPors.Api.Controllers;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.User;

namespace VakilPors.Test.Api.Controllers;

using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Case;
using VakilPors.Shared.Response;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using VakilPors.Core.Domain.Entities;

public class DocumentControllerTests
{
    private readonly Mock<ILegalDocumentService> _mockDocumentService = new Mock<ILegalDocumentService>();
    private readonly DocumentController _controller;

    public DocumentControllerTests()
    {
        _controller = new DocumentController(_mockDocumentService.Object);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal() }
        };
    }

    private ClaimsPrincipal CreateClaimsPrincipal()
    {
        var claims = new[] { new Claim("uid", "1") };
        var identity = new ClaimsIdentity(claims, "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        return claimsPrincipal;
    } 
  
    [Fact]
    public async Task AddDocument_ShouldCallAddDocumentAndReturnSuccess()
    {
        // Arrange
        var documentDto = new LegalDocumentDto();
        var expectedResponse = documentDto;
        _mockDocumentService.Setup(service => service.AddDocument(It.IsAny<int>(), documentDto))
                            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.AddDocument(documentDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<AppResponse<object>>(okResult.Value);
        Assert.Equal("success", response.Message);
        Assert.Equal(expectedResponse, response.Data);
    }

    // The same pattern can be applied for UpdateDocument, DeleteDocument, and the other methods.

    // Since UpdateDocumentStatus involves Authorize attribute with Roles, further setup will be needed
    // to mock User.Identity.IsAuthenticated and User.IsInRole.

    [Fact]
    public async Task UpdateDocumentStatus_WhenUserIsVakil_ShouldUpdateStatusAndReturnSuccess()
    {
        // Arrange
        var updateDto = new DocumentStatusUpdateDto();
        _mockDocumentService.Setup(service => service.UpdateDocumentStatus(updateDto, It.IsAny<int>()))
                            .Returns(Task.CompletedTask);

        // Additional mock setup may be required here to mock the user's role as Vakil.

        // Act
        var result = await _controller.UpdateDocumentStatus(updateDto);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
    
    [Fact]
    public async Task DeleteDocument_ShouldCallServiceAndReturnSuccess()
    {
        // Arrange
        int documentId = 5; 
        bool serviceReturnValue = true; // Assuming service returns a boolean for successful deletion
        _mockDocumentService.Setup(service => service.DeleteDocument(documentId))
                            .ReturnsAsync(serviceReturnValue);

        // Act
        var result = await _controller.DeleteDocument(documentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<AppResponse<object>>(okResult.Value);
        Assert.Equal("success", response.Message);
        Assert.Equal(serviceReturnValue, response.Data);
    }

    [Fact]
    public async Task GetDocumentsByUserId_ShouldReturnDocuments()
    {
        // Arrange
        var userId = 1;
        var documentStatus = Status.PENDING; // Assuming 'Status' is an enum
        var pagedParams = new PagedParams { PageNumber = 1, PageSize = 10 };
        var expectedDocuments = new Pagination<LegalDocument>(new List<LegalDocument>(),pagedParams.PageSize); // Replace with actual expected document list
        
        _mockDocumentService.Setup(service => service.GetDocumentsByUserId(userId, documentStatus, pagedParams))
                            .ReturnsAsync(expectedDocuments);

        // Act
        var result = await _controller.GetDocumentsByUserId(userId, documentStatus, pagedParams);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<AppResponse<object>>(okResult.Value);
        Assert.Equal("success", response.Message);
        Assert.Equal(expectedDocuments, response.Data);
    }

    [Fact]
    public async Task GetDocumentById_ShouldReturnDocument()
    {
        // Arrange
        int documentId = 3;
        var expectedDocument = new LegalDocument { /* Set the expected properties */ };
        _mockDocumentService.Setup(service => service.GetDocumentById(documentId))
                            .ReturnsAsync(expectedDocument);

        // Act
        var result = await _controller.GetDocumentById(documentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<AppResponse<object>>(okResult.Value);
        Assert.Equal("success", response.Message);
        Assert.Equal(expectedDocument, response.Data);
    }

    [Fact]
    public async Task GrantAccessToLawyer_ShouldGrantAccessAndReturnSuccess()
    {
        // Arrange
        var documentAccessDto = new DocumentAccessDto(); // Assuming the object contains necessary information
        bool serviceReturnValue = true; // Assuming the service method returns a boolean for success
        _mockDocumentService.Setup(service => service.GrantAccessToLawyer(documentAccessDto))
                            .ReturnsAsync(serviceReturnValue);

        // Act
        var result = await _controller.GrantAccessToLawyer(documentAccessDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<AppResponse<object>>(okResult.Value);
        Assert.Equal("success", response.Message);
        Assert.Equal(serviceReturnValue, response.Data);
    }

    [Fact]
    public async Task TakeAccessFromLawyer_ShouldRevokeAccessAndReturnSuccess()
    {
        // Arrange
        var documentAccessDto = new DocumentAccessDto(); // Assuming necessary information is set
        bool serviceReturnValue = true; // Assuming the service method returns a boolean
        _mockDocumentService.Setup(service => service.TakeAccessFromLawyer(documentAccessDto))
                            .ReturnsAsync(serviceReturnValue);

        // Act
        var result = await _controller.TakeAccessFromLawyer(documentAccessDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<AppResponse<object>>(okResult.Value);
        Assert.Equal("success", response.Message);
        Assert.Equal(serviceReturnValue, response.Data);
    }

    [Fact]
    public async Task GetLawyersThatHaveAccessToDocument_ShouldReturnLawyers()
    {
        // Arrange
        int documentId = 2;
        var expectedLawyers = new List<LawyerDto>(); // Replace with actual expected lawyers list
        _mockDocumentService.Setup(service => service.GetLawyersThatHaveAccessToDocument(documentId))
                            .ReturnsAsync(expectedLawyers);

        // Act
        var result = await _controller.GetLawyersThatHaveAccessToDocument(documentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<AppResponse<object>>(okResult.Value);
        Assert.Equal("success", response.Message);
        Assert.Equal(expectedLawyers, response.Data);
    }

    [Fact]
    public async Task GetUsersThatLawyerHasAccessToTheirDocuments_ShouldReturnUsers()
    {
        // Arrange
        int lawyerId = 1;
        var expectedUsers = new List<UserDto>(); // Replace with your expected user list
        _mockDocumentService.Setup(service => service.GetUsersThatLawyerHasAccessToTheirDocuments(lawyerId))
                            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _controller.GetUsersThatLawyerHasAccessToTheirDocuments(lawyerId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<AppResponse<object>>(okResult.Value);
        Assert.Equal("success", response.Message);
        Assert.Equal(expectedUsers, response.Data);
    }

    [Fact]
    public async Task GetDocumentsThatLawyerHasAccessToByUserId_ShouldReturnDocuments()
    {
        // Arrange
        var lawyerDocumentAccessDto = new LawyerDocumentAccessDto();
        var documentStatus = Status.ACCEPTED; // Assuming 'Status' is an enum
        var expectedDocuments = new List<LegalDocument>(); // Replace with actual list

        _mockDocumentService.Setup(service => 
            service.GetDocumentsThatLawyerHasAccessToByUserId(lawyerDocumentAccessDto, documentStatus))
                            .ReturnsAsync(expectedDocuments);

        // Act
        var result = await _controller.GetDocumentsThatLawyerHasAccessToByUserId(lawyerDocumentAccessDto, documentStatus);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<AppResponse<object>>(okResult.Value);
        Assert.Equal("success", response.Message);
        Assert.Equal(expectedDocuments, response.Data);
    }

    [Fact]
    public async Task UpdateDocument_ReturnsOkResultWithSuccessMessage()
    {
        // Arrange
        var documentDto = new LegalDocumentDto
        {
            // Assume that all necessary properties are set up here
        };
        var returnedDocument = new LegalDocumentDto
        {
            // This is the data we expect to be returned from the update method
            // It can be the same 'documentDto' object or a modified version
            // depending on your service layer logic
        };
        _mockDocumentService.Setup(service => service.UpdateDocument(documentDto))
            .ReturnsAsync(returnedDocument);

        // Act
        var result = await _controller.UpdateDocument(documentDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<AppResponse<object>>(okResult.Value);
        Assert.Equal("success", response.Message);
        Assert.Equal(returnedDocument, response.Data);
        
        // Optionally, you may want to check if the service method was called with the expected parameters
        _mockDocumentService.Verify(service => service.UpdateDocument(documentDto), Times.Once());
    }
}