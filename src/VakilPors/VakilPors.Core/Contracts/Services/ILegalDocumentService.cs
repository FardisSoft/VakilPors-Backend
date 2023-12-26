

using System.Reflection.Metadata;
using Pagination.EntityFrameworkCore.Extensions;
using VakilPors.Core.Domain.Dtos.Case;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services
{
    public interface ILegalDocumentService : IScopedDependency
    {
        Task<LegalDocumentDto> AddDocument(int userId, LegalDocumentDto documentDto);

        Task<LegalDocumentDto> UpdateDocument(LegalDocumentDto documentDto);

        Task<bool> DeleteDocument(int documentId);

        Task<Pagination<LegalDocument>> GetDocumentsByUserId(int userId, Status? status, PagedParams pagedParams);

        Task<LegalDocument> GetDocumentById(int documentId);

        Task<bool> GrantAccessToLawyer(DocumentAccessDto documentAccessDto);

        Task<bool> TakeAccessFromLawyer(DocumentAccessDto documentAccessDto);

        Task<List<LawyerDto>> GetLawyersThatHaveAccessToDocument(int documentId);

        Task<List<UserDto>> GetUsersThatLawyerHasAccessToTheirDocuments(int lawyerId);

        Task<List<LegalDocument>> GetDocumentsThatLawyerHasAccessToByUserId(
            LawyerDocumentAccessDto lawyerDocumentAccessDto,Status? status);
        Task UpdateDocumentStatus(DocumentStatusUpdateDto updateDto, int lawyerUserId);
    }
}
