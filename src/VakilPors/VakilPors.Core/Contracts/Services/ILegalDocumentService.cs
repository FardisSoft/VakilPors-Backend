

using System.Reflection.Metadata;
using VakilPors.Core.Domain.Dtos.Case;
using VakilPors.Core.Domain.Dtos.Lawyer;
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

        Task<List<LegalDocumentDto>> GetDocumentsByUserId(int userId);

        Task<LegalDocumentDto> GetDocumentById(int documentId);

        Task<bool> GrantAccessToLawyer(DocumentAccessDto documentAccessDto);

        Task<bool> TakeAccessFromLawyer(DocumentAccessDto documentAccessDto);

        Task<List<LawyerDto>> GetLawyersThatHaveAccessToDocument(int documentId);

        Task<List<UserDto>> GetUsersThatLawyerHasAccessToTheirDocuments(int lawyerId);

        Task<List<LegalDocumentDto>> GetDocumentsThatLawyerHasAccessToByUserId(LawyerDocumentAccessDto lawyerDocumentAccessDto);
    }
}
