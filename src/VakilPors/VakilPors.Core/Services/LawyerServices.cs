using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FuzzySharp;
using Microsoft.EntityFrameworkCore;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Params;
using X.PagedList;
using VakilPors.Shared.Extensions;
using VakilPors.Core.Domain.Entities;

namespace VakilPors.Core.Services
{
    public class LawyerServices : ILawyerServices
    {
        private readonly IAppUnitOfWork appUnitOfWork;
        public LawyerServices(IAppUnitOfWork appUnitOfWork)
        {
            this.appUnitOfWork = appUnitOfWork;

        }
        public async Task<IPagedList<Lawyer>> GetLawyers(PagedParams pagedParams, FilterParams filterParams)
        {
            return await appUnitOfWork.LawyerRepo.AsQueryableNoTracking()
            .Include(l => l.User)
            .Where(l => string.IsNullOrEmpty(filterParams.Q) || Fuzz.PartialRatio(l.User.Name, filterParams.Q) > 75 || l.ParvandeNo.Contains(filterParams.Q))
            .OrderBy((string.IsNullOrEmpty(filterParams.Sort) ? "Id" : filterParams.Sort), filterParams.IsAscending)
            .ToPagedListAsync(pagedParams.PageNumber, pagedParams.PageSize);
        }
    }
}