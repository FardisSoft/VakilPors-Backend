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

        public async Task ActivatePremium(SubscribedDto premium, int user_id)
        {
            DateTime expdate = DateTime.Now;
            DateTime now = DateTime.Today;
            int money = 0;
            switch (premium.Premium.ServiceType)
            {
                 case Plan.Bronze:
                    expdate = now.AddDays(30);
                    money = 20000;
                    break;
                case Plan.Siler:
                    expdate = now.AddDays(60);
                    money = 30000;
                    break;
                case Plan.Gold:
                    expdate = now.AddDays(60);
                    money = 50000;
                    break;
            }
            premium.ExpireDate = expdate;
            premium.User.Balance -= money;
            var subscribed = _mapper.Map<Subscribed>(premium);
            await _appUnitOfWork.SubscribedRepo.AddAsync(subscribed);
            await _appUnitOfWork.SaveChangesAsync();
        }

        public async Task DeactivatePremium(int user_id)
        {
            var subscribed = await _appUnitOfWork.SubscribedRepo.FindAsync(user_id);
            subscribed.ExpireDate = DateTime.Now;
            _appUnitOfWork.SubscribedRepo.Update(subscribed);
            await _appUnitOfWork.SaveChangesAsync();    
        }
        public async Task UpdatePlan(SubscribedDto subscribedDto)
        {
            var sub = await _appUnitOfWork.SubscribedRepo.FindAsync(subscribedDto.ID);
            sub.Premium.ServiceType = subscribedDto.Premium.ServiceType;
            _appUnitOfWork.SubscribedRepo.Update(sub);
            await _appUnitOfWork.SaveChangesAsync();
        }
        public async Task<SubscribedDto> GetPremiumStatus(int user_id)
        {
            var subscribed = await _appUnitOfWork.SubscribedRepo.AsQueryable().Where(x => x.UserId == user_id).FirstOrDefaultAsync();
            var subdto = _mapper.Map<SubscribedDto>(subscribed);
            return subdto;

        }
    }
}
