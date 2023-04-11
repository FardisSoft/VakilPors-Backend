using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Services;
using X.PagedList;

namespace VakilPors.Core.Contracts.Services
{
    public interface ILawyerServices:IScopedDependency
    {
        Task<IPagedList<Lawyer>> GetLawyers(PagedParams pagedParams, FilterParams filterParams);
        Task <Lawyer> GetLawyerByID (int id);
    }
}