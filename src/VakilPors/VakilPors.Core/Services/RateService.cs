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
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;

namespace VakilPors.Core.Services
{
    public class RateService : IRateService
    {
        private readonly IMapper _mapper;
        private readonly IAppUnitOfWork _appUnitOfWork;
        private readonly ILawyerServices _lawyerServices;

        public RateService(IMapper mapper, IAppUnitOfWork appUnitOfWork, ILawyerServices lawyerServices)
        {
            _mapper = mapper;
            _appUnitOfWork = appUnitOfWork;
            _lawyerServices = lawyerServices;
        }

        public async Task AddRateAsync(RateDto rate, int user_id, int lawyer_id)
        {
            var ratee = await _appUnitOfWork.RateRepo.AsQueryable().Where(x=>x.LawyerId == lawyer_id && x.UserId == user_id).FirstOrDefaultAsync();
            if (ratee != null)
            {
                throw new BadArgumentException("Has rated before!");
            }
            var raate  = _mapper.Map<Rate>(rate);
            raate.UserId = user_id;
            raate.LawyerId = lawyer_id;
            await _appUnitOfWork.RateRepo.AddAsync(raate);
            var lawyer = _appUnitOfWork.LawyerRepo.AsQueryable().Where(x => x.Id == lawyer_id).FirstOrDefault();
            if(lawyer.Rating == 0)
            {
                lawyer.Rating += rate.RateNum;
            }
            else
            {
                var avg = ((lawyer.NumberOfRates * lawyer.Rating) + rate.RateNum) / (lawyer.NumberOfRates + 1);
                lawyer.Rating = avg;
            }
            lawyer.NumberOfRates += 1;

            await _lawyerServices.AddToken(lawyer_id,  (int)Math.Floor(rate.RateNum));

            await _appUnitOfWork.SaveChangesAsync();
        }

        public async Task<double> CalculateRatingAsync(int laywer_id)
        {
            var rates = await _appUnitOfWork.RateRepo.AsQueryable().Where(x=>x.LawyerId == laywer_id).ToListAsync();
            double avg = 0, count = 0, sum = 0;
            if (rates.Count > 0)
            {
                foreach (var rate in rates)
                {
                    sum += rate.RateNum;
                    count++;
                }

                avg = sum / count;
            }
            return avg;
        }

        public async Task DeleteRateAsync(int rate_id)
        {
            var rate = await _appUnitOfWork.RateRepo.FindAsync(rate_id);
            _appUnitOfWork.RateRepo.Remove(rate);
            await _appUnitOfWork.SaveChangesAsync();
        }

        public async Task<List<RateUserDto>> GetAllRatesAsync(int lawyer_id) 
        {
            List<Rate> rates = new List<Rate>();
            rates = await _appUnitOfWork.RateRepo.AsQueryable().Where(x=> x.LawyerId == lawyer_id).ToListAsync();
            List<RateUserDto> _rates = new List<RateUserDto>();
            foreach (var rate in rates) 
            {
                RateUserDto r = new RateUserDto();
                var rate_row = await _appUnitOfWork.RateRepo.FindAsync(rate.Id);
                var uuser = await _appUnitOfWork.UserRepo.FindAsync(rate_row.UserId);
                r.user = _mapper.Map<UserDto>(uuser);
                r.RateNum = rate.RateNum;
                r.Comment = rate.Comment;
                _rates.Add(r);  
            }
            if (_rates.Count == 0)
            {
                throw new BadArgumentException("NO RATES FOUND!");
            }
            return _rates;
        }

        public async Task<RateDto> GetRateAsync(int user_id, int lawyer_id)
        {
            var rate = await _appUnitOfWork.RateRepo.AsQueryable().Where(x => x.UserId == user_id && x.LawyerId == lawyer_id).FirstOrDefaultAsync();
            var rate_dto = _mapper.Map<RateDto>(rate);
            return rate_dto;
        }

        public async Task UpdateRateAsync(RateDto rate, int user_id, int lawyer_id)
        {
            var ratte = await _appUnitOfWork.RateRepo.AsQueryable().Where(x=>x.UserId == user_id && x.LawyerId == lawyer_id).FirstOrDefaultAsync();
            if (ratte == null)
            {
                throw new BadArgumentException("Rate Not Found");
            }
            ratte.Comment = rate.Comment;
            ratte.RateNum = rate.RateNum;
            _appUnitOfWork.RateRepo.Update(ratte);
            var user_rate = await GetRateAsync(user_id, lawyer_id);
            var lawyer = _appUnitOfWork.LawyerRepo.AsQueryable().Where(x => x.Id == lawyer_id).FirstOrDefault();
            lawyer.Rating = ((lawyer.NumberOfRates * lawyer.Rating) - user_rate.RateNum) / lawyer.NumberOfRates-1;
            lawyer.Rating = ((lawyer.NumberOfRates * lawyer.Rating) + rate.RateNum) / lawyer.NumberOfRates ;
            await _appUnitOfWork.SaveChangesAsync();
            
        }
    }
}
