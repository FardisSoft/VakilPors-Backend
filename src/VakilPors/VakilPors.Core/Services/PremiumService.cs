using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FuzzySharp;
using Microsoft.EntityFrameworkCore;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.User;
using X.PagedList;
using VakilPors.Shared.Extensions;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;
using VakilPors.Core.Domain.Dtos.Premium;

namespace VakilPors.Core.Services
{
    public class PremiumService : IPremiumService
    {
        private readonly IAppUnitOfWork _appUnitOfWork;
        private readonly IMapper _mapper;

        public PremiumService(IAppUnitOfWork appUnitOfWork, IMapper mapper)
        {
            _appUnitOfWork = appUnitOfWork;
            _mapper = mapper;
        }


        public async Task ActivatePremium(PremiumDto premium, int user_id)
        {
            var pr = _mapper.Map<Premium>(premium);
            await _appUnitOfWork.PremiumRepo.AddAsync(pr);
        }

        public async Task DeactivatePremium(int user_id)
        {
            var foundpremium = await _appUnitOfWork.PremiumRepo.FindAsync(user_id);
            if (foundpremium == null)
                throw new BadArgumentException("Lawyer Not Found");
            foundpremium.IsExpired = true;

        }

        public async Task<PremiumDto> GetPremiumStatus(int user_id)
        {
            var _premium = await _appUnitOfWork.PremiumRepo.FindAsync(user_id);
            var premium = _mapper.Map<PremiumDto>(_premium);
            return premium;
        }
    }
}
