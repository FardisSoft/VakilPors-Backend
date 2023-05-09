using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Rate;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;

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

        public async Task AddRateAsync(RateDto rate, int user_id, int lawyer_id)
        {
            var ratee = await _appUnitOfWork.RateRepo.FindAsync(rate.Id);
            if (ratee != null)
            {
                throw new BadArgumentException("Has rated before!");
            }
            var raate  = _mapper.Map<Rate>(rate);
            raate.UserId = user_id;
            raate.LawyerId = lawyer_id;
            await _appUnitOfWork.RateRepo.AddAsync(raate);
            await _appUnitOfWork.SaveChangesAsync();
        }

        public async Task<double> CalculateRatingAsync(int laywer_id)
        {
            var rates = await GetAllRatesAsync(laywer_id);
            double avg = 0, count = 0, sum = 0;
            foreach (var rate in rates) 
            {
                sum += rate.RateNum;
                count++;
            }
            avg = sum / count;
            return avg;
        }

        public async Task DeleteRateAsync(int rate_id)
        {
            var rate = await _appUnitOfWork.RateRepo.FindAsync(rate_id);
            _appUnitOfWork.RateRepo.Remove(rate);
            await _appUnitOfWork.SaveChangesAsync();
        }

        public async Task<List<Rate>> GetAllRatesAsync(int lawyer_id)
        {
            List<Rate> rates = new List<Rate>();
            rates = await _appUnitOfWork.RateRepo.AsQueryable().Where(x=> x.LawyerId == lawyer_id).ToListAsync();
            return rates;
        }

        public async Task<RateDto> GetRateAsync(int user_id, int lawyer_id)
        {
            var rate = await _appUnitOfWork.RateRepo.AsQueryable().Where(x => x.UserId == user_id && x.LawyerId == lawyer_id).FirstOrDefaultAsync();
            var rate_dto = _mapper.Map<RateDto>(rate);
            return rate_dto;
        }

        public async Task UpdateRateAsync(RateDto rate)
        {
            var ratte = await _appUnitOfWork.RateRepo.FindAsync(rate.Id);
            if (ratte == null)
            {
                throw new BadArgumentException("Rate Not Found");
            }
            ratte.Comment = rate.Comment;
            ratte.RateNum = rate.RateNum;
            _appUnitOfWork.RateRepo.Update(ratte);
            await _appUnitOfWork.SaveChangesAsync();
            
        }
    }
}
