using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pagination.EntityFrameworkCore.Extensions;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.Search;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Services;
using X.PagedList;

namespace VakilPors.Core.Contracts.Services
{
    public interface ILawyerServices:IScopedDependency
    {
        Task<Pagination<Lawyer>> GetLawyers(PagedParams pagedParams, SortParams sortParams , LawyerFilterParams filterParams);
        Task<LawyerDto> UpdateLawyer(LawyerDto lawyerDto);
        Task<List<LawyerDto>> GetAllLawyers();
        Task<LawyerDto> GetLawyerById(int lawyerId);
        Task<LawyerDto> GetLawyerByUserId(int userId);
        Task AddToken(int lawyerId, int tokens);
        Task<bool> TransferToken(int lawyerId);
        Task<bool> VerifyLawyer(int lawyerId);
        Task<bool> IsLawyer(int userId);
        //Task<List<LawyerDto>> FilteredSearch(SearchDto lawyerDto);
        public Task<List<LawyerCityCountDto>> GetLawyerCityCounts();
        public Task<List<LawyerTitleCountDto>> GetLawyerTitleCounts();
        public Task<Pagination<Lawyer>> GetAllUnverfiedLawyers(PagedParams pagedParams, SortParams sortParams);
    }
}