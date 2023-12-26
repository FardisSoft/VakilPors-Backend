using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Case;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Response;

namespace VakilPors.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class DocumentController : MyControllerBase
    {
        private readonly ILegalDocumentService _documentService;

        public DocumentController(ILegalDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost]
        public async Task<IActionResult> AddDocument([FromForm] LegalDocumentDto documentDto)
        {
            var result = await _documentService.AddDocument(GetUserId(), documentDto);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDocument([FromForm] LegalDocumentDto documentDto)
        {
            var result = await _documentService.UpdateDocument(documentDto);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteDocument(int documentId)
        {
            var result = await _documentService.DeleteDocument(documentId);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> GetDocumentsByUserId(int userId,[FromQuery]Status? documentStatus,[FromQuery]PagedParams pagedParams)
        {
            var result = await _documentService.GetDocumentsByUserId(userId,documentStatus,pagedParams);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> GetDocumentById(int documentId)
        {
            var result = await _documentService.GetDocumentById(documentId);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpPost]
        public async Task<IActionResult> GrantAccessToLawyer(DocumentAccessDto documentAccessDto)
        {
            var result = await _documentService.GrantAccessToLawyer(documentAccessDto);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpPost]
        public async Task<IActionResult> TakeAccessFromLawyer(DocumentAccessDto documentAccessDto)
        {
            var result = await _documentService.TakeAccessFromLawyer(documentAccessDto);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> GetLawyersThatHaveAccessToDocument(int documentId)
        {
            var result = await _documentService.GetLawyersThatHaveAccessToDocument(documentId);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersThatLawyerHasAccessToTheirDocuments(int lawyerId)
        {
            var result = await _documentService.GetUsersThatLawyerHasAccessToTheirDocuments(lawyerId);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpPost]
        public async Task<IActionResult> GetDocumentsThatLawyerHasAccessToByUserId(LawyerDocumentAccessDto lawyerDocumentAccessDto,[FromQuery]Status? documentStatus)
        {
            var result = await _documentService.GetDocumentsThatLawyerHasAccessToByUserId(lawyerDocumentAccessDto,documentStatus);
            return Ok(new AppResponse<object>(result, "success"));
        }
        [HttpPatch]
        [Authorize(Roles = RoleNames.Vakil)]
        public async Task<IActionResult> UpdateDocumentStatus([FromBody] DocumentStatusUpdateDto updateDto)
        {
            var userId = GetUserId();
            await _documentService.UpdateDocumentStatus(updateDto,userId);
            return Ok("success");
        }

    }
}
