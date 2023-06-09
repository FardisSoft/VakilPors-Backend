using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Services;
using X.PagedList;

namespace VakilPors.Core.Contracts.Services
{
    public interface ILawyerServices:IScopedDependency
    {
        Task<IPagedList<Lawyer>> GetLawyers(PagedParams pagedParams, FilterParams filterParams);

        Task<LawyerDto> UpdateLawyer(LawyerDto lawyerDto);
        Task<List<LawyerDto>> GetAllLawyers();
        Task<LawyerDto> GetLawyerById(int lawyerId);
        Task<LawyerDto> GetLawyerByUserId(int userId);
        Task AddToken(int lawyerId, int tokens);
        Task<bool> TransferToken(int lawyerId);
        Task<bool> VerifyLawyer(int lawyerId);
        Task<bool> IsLawyer(int userId);
    }
}