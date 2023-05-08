using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Rate;
using VakilPors.Core.Domain.Entities;

namespace VakilPors.Core.Services
{
    public class RateService : IRateService
    {
        private readonly IMapper _mapper;
        private readonly IAppUnitOfWork _appUnitOfWork;

        public RateService(IMapper mapper, IAppUnitOfWork appUnitOfWork)
        {
            _mapper = mapper;
            _appUnitOfWork = appUnitOfWork;
        }

        public Task AddRateAsync(RateDto rate, int user_id, int lawyer_id)
        {
            throw new NotImplementedException();
        }

        public Task<double> CalculateRatingAsync(int laywer_id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRateAsync(int user_id, int lawyer_id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Rate>> GetAllRatesAsync(int lawyer_id)
        {
            throw new NotImplementedException();
        }

        public Task<RateDto> GetRateAsync(int user_id, int lawyer_id)
        {
            throw new NotImplementedException();

        }

        public Task UpdateRateAsync(RateDto rate, int user_id, int lawyer_id)
        {
            throw new NotImplementedException();
        }
    }
}
