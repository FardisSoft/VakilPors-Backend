using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;
using X.PagedList;

namespace VakilPors.Core.Services
{
    public class WalletServices : IWalletServices
    {
        private readonly UserManager<User> userManager;
        private readonly IAppUnitOfWork appUnitOfWork;

        public WalletServices(UserManager<User> userManager, IAppUnitOfWork appUnitOfWork)
        {
            this.userManager = userManager;
            this.appUnitOfWork = appUnitOfWork;
        }
        public async Task AddBalance(string phoneNumber, decimal amount)
        {
            var user=await getUser(phoneNumber);
            user.Balance += amount;
            var result=await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InternalServerException("error in update user balance");
            }
        }

        public async Task AddBalance(int userId, decimal amount)
        {
            var user=await getUser(userId);
            user.Balance += amount;
            var result=await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InternalServerException("error in update user balance");
            }
        }

        public async Task AddTransaction(int userId, string phoneNumber, decimal amount, string description,string authority, bool isSuccess, bool isIncome)
        {
            var tranaction=new Tranaction(){
                UserId=userId,
                Amount=amount,
                Date=DateTime.Now,
                Description=description,
                Authority=authority,
                IsIncome=isIncome,
                IsSuccess=isSuccess,
            };
            await appUnitOfWork.TransactionRepo.AddAsync(tranaction);
            await appUnitOfWork.SaveChangesAsync();
            if(isSuccess)
                await AddBalance(phoneNumber, (isIncome?amount:-amount));
        }
        

        public async Task<decimal> GetBalance(string phoneNumber)
        {
            var user=await getUser(phoneNumber);
            return user.Balance;
        }
        public async Task<IPagedList<Tranaction>> GetTransactions(string phoneNumber,PagedParams pagedParams)
        {
            var tranactions=await appUnitOfWork.UserRepo.AsQueryableNoTracking().Include(u=>u.Tranactions).Where(x=>x.PhoneNumber==phoneNumber).Select(x=>x.Tranactions).ToPagedListAsync(pagedParams.PageNumber, pagedParams.PageSize);
            return tranactions.FirstOrDefault().ToPagedList();
        }

        
        private async Task<User> getUser(string phoneNumber)
        {
            var user= await userManager.FindByNameAsync(phoneNumber);
            if (user == null)
            {
                throw new NotFoundException("user not found");
            }
            return user;
        }
        private async Task<User> getUser(int userId)
        {
            var user= await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException("user not found");
            }
            return user;
        }
    }
}