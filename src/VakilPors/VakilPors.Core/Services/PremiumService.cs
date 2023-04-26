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
        //private readonly IPremiumService _premiumservice;

        public PremiumService(IAppUnitOfWork appUnitOfWork, IMapper mapper)
        {
            _appUnitOfWork = appUnitOfWork;
            _mapper = mapper;
            //_premiumservice = premiumservice;
        
        }

        public Task ActivatePremium(PremiumDto premium, int user_id)
        {
            throw new NotImplementedException();
        }

        public Task DeactivatePremium(int user_id)
        {
            throw new NotImplementedException();
        }

        public Task<PremiumDto> GetPremiumStatus(int user_id)
        {
            throw new NotImplementedException();
        }
    }
}
