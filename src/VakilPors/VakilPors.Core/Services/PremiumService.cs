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
using VakilPors.Core.Services;
using Pagination.EntityFrameworkCore.Extensions;

namespace VakilPors.Core.Services
{
    public class PremiumService : IPremiumService
    {
        private readonly IAppUnitOfWork _appUnitOfWork;
        private readonly IMapper _mapper;
        private readonly IWalletServices _walletservice;
        //private readonly IPremiumService _premiumservice;

        public PremiumService(IAppUnitOfWork appUnitOfWork, IMapper mapper, IWalletServices walletservice)
        {
            _appUnitOfWork = appUnitOfWork;
            _mapper = mapper;
            _walletservice = walletservice;
            //_premiumservice = premiumservice;

        }

        public async Task<Subscribed> ActivatePremium(string premium, int user_id)
        {
            var row = _appUnitOfWork.SubscribedRepo.AsQueryable().Where(x => x.UserId == user_id).First();
            if (row == null)
                throw new BadArgumentException("Subscription Not Found");
            switch (premium)
            {
                case "gold":
                    row.PremiumID = 3;
                    await TransactUser("gold", user_id, 50000, "طلایی");
                    row.ExpireDate = DateTime.Now.AddDays(90);
                    break;
                case "silver":
                    row.PremiumID = 2;
                    await TransactUser("silver", user_id, 30000, "نقره ای");
                    row.ExpireDate = DateTime.Now.AddDays(60);
                    break;
                case "bronze":
                    await TransactUser("bronze", user_id, 20000, "برنزی");
                    row.ExpireDate = DateTime.Now.AddDays(30);
                    row.PremiumID = 1;
                    break;
            }
            await _appUnitOfWork.SaveChangesAsync();
            return row;

        }

        public async Task TransactUser(string v, int user_id, int amount, string baste)
        {
            var user = await _appUnitOfWork.UserRepo.FindAsync(user_id);
            await _walletservice.AddTransaction(user_id, amount, $"خرید بسته {baste}", " ", true, false);

        }

        public async Task DeactivatePremium(int user_id)
        {
            //var subscribed = await _appUnitOfWork.SubscribedRepo.FindAsync(user_id);
            //subscribed.ExpireDate = DateTime.Now;
            //_appUnitOfWork.SubscribedRepo.Update(subscribed);
            //await _appUnitOfWork.SaveChangesAsync();    
            var sub = await _appUnitOfWork.SubscribedRepo.AsQueryable().Where(x => x.UserId == user_id).FirstAsync();
            if (sub == null)
                throw new BadArgumentException("Subscription Not Found");
            sub.PremiumID = 1;
            sub.ExpireDate = DateTime.MaxValue;
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
            var subscribed = await _appUnitOfWork.SubscribedRepo.AsQueryable().Include(x => x.Premium).Where(x => x.UserId == user_id).FirstOrDefaultAsync();
            var subdto = _mapper.Map<SubscribedDto>(subscribed);
            return subdto;

        }

        public async Task<bool> DoseUserHaveAnyActiveSubscription(int userId)
        {
            var sub = await GetPremiumStatus(userId);
            bool isPremium =
                sub is { IsExpired: false } &&
                sub.Premium?.ServiceType != Plan.Free;

            return isPremium;
        }
        public async Task<Pagination<Subscribed>> GetAllSubscriptionStatus(PagedParams pagedParams, SortParams sortParams)
        {
            var all_subs = _appUnitOfWork.SubscribedRepo.AsQueryable().Include(x => x.User).Where(x => x.PremiumID > 1);
            return await all_subs.AsPaginationAsync(pagedParams.PageNumber, pagedParams.PageSize);

        }
        
    }

}
