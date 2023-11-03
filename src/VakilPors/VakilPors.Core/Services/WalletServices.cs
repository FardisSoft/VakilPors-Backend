using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pagination.EntityFrameworkCore.Extensions;
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
        private readonly IEmailSender emailSender;
        private readonly ITelegramService _telegramService;

        public WalletServices(UserManager<User> userManager, IAppUnitOfWork appUnitOfWork, IEmailSender emailSender,
            ITelegramService telegramService)
        {
            this.userManager = userManager;
            this.appUnitOfWork = appUnitOfWork;
            this.emailSender = emailSender;
            this._telegramService = telegramService;
        }

        public async Task AddBalance(string phoneNumber, decimal amount)
        {
            var user = await getUser(phoneNumber);
            await addBalance(user, amount);
        }

        public async Task AddBalance(int userId, decimal amount)
        {
            var user = await getUser(userId);
            await addBalance(user, amount);
        }

        private async Task addBalance(User user, decimal amount)
        {
            user.Balance += amount;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InternalServerException("error in update user balance");
            }
        }


        public async Task AddTransaction(int userId, decimal amount, string description, string authority,
            bool isSuccess, bool isIncome, bool isWithdraw = false)
        {
            var transaction = new Transaction()
            {
                UserId = userId,
                Amount = amount,
                Date = DateTime.Now,
                Description = description,
                Authority = authority,
                IsIncome = isIncome,
                IsSuccess = isSuccess,
                IsWithdraw = isWithdraw,
            };
            await appUnitOfWork.TransactionRepo.AddAsync(transaction);
            await appUnitOfWork.SaveChangesAsync();
            if (isSuccess)
                await AddBalance(userId, (isIncome ? amount : -amount));
            string yesorno = isSuccess ? "بود" : "نبود";
            string body =
                $@"تراکنش شما در تاریخ {DateTime.Now} ثبت شد.مبلغ: {amount} توضیحات: {description} کد تراکنش: {authority}  تراکنش موفقیت آمیز {yesorno}.";
            
            var user = await getUser(userId);
            await emailSender.SendEmailAsync(user.Email, user.Name, "تراکنش", body);
            if (user.Telegram != null)
            {
                await _telegramService.SendToTelegram(body, user.Telegram);
            }
        }

        public async Task ApproveTransaction(int transactionId)
        {
            var transaction = await appUnitOfWork.TransactionRepo.FindAsync(transactionId);
            if (transaction.IsSuccess)
            {
                throw new InternalServerException("transaction is already applied");
            }

            transaction.IsSuccess = true;
            appUnitOfWork.TransactionRepo.Update(transaction);
            await appUnitOfWork.SaveChangesAsync();
            await AddBalance(transaction.UserId, (transaction.IsIncome ? transaction.Amount : -transaction.Amount));
        }

        public async Task ApplyTransaction(int transactionId)
        {
            var transaction = await appUnitOfWork.TransactionRepo.AsQueryable().Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == transactionId);
            var user = transaction.User;
            if (!transaction.IsSuccess || (!transaction.IsIncome && user.Balance < transaction.Amount))
            {
                return;
            }

            var amount = (transaction.IsIncome ? transaction.Amount : -transaction.Amount);
            user.Balance += amount;
            appUnitOfWork.UserRepo.Update(user);
            await appUnitOfWork.SaveChangesAsync();
        }


        public async Task<decimal> GetBalance(string phoneNumber)
        {
            var user = await getUser(phoneNumber);
            return user.Balance;
        }

        public async Task<Pagination<Transaction>> GetTransactions(string phoneNumber, PagedParams pagedParams)
        {
            var transactions = await appUnitOfWork.UserRepo.AsQueryableNoTracking()
                .Include(u => u.Transactions)
                .Where(x => x.PhoneNumber == phoneNumber)
                .SelectMany(x => x.Transactions)
                .AsPaginationAsync(pagedParams.PageNumber, pagedParams.PageSize);
            return transactions;
        }

        public async Task<IEnumerable<Transaction>> GetWithdrawTransactions()
        {
            var transactions = appUnitOfWork.TransactionRepo.AsQueryableNoTracking().Where(x => x.IsWithdraw);
            return await transactions.ToArrayAsync();
        }

        public async Task Withdraw(int userId, decimal amount, string cardNo)
        {
            var user = await getUser(userId);
            if (user.Balance < amount)
            {
                throw new BadArgumentException("Not enough balance");
            }

            await AddTransaction(userId, amount, $"برداشت از کیف پول، شماره کارت:{cardNo}", "", true, false, true);
            await userManager.UpdateAsync(user);
        }

        public async Task PayWithdraw(int transactionId)
        {
            var trans = await appUnitOfWork.TransactionRepo.FindAsync(transactionId);
            if (!trans.IsWithdraw)
            {
                throw new BadArgumentException("The transaction is not withdraw!");
            }

            if (trans.IsPaid)
            {
                throw new BadArgumentException("The transaction is already paid!");
            }

            trans.IsPaid = true;
            appUnitOfWork.TransactionRepo.Update(trans);
            await appUnitOfWork.SaveChangesAsync();
        }

        private async Task<User> getUser(string phoneNumber)
        {
            var user = await userManager.FindByNameAsync(phoneNumber);
            if (user == null)
            {
                throw new NotFoundException("user not found");
            }

            return user;
        }

        private async Task<User> getUser(int userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException("user not found");
            }

            return user;
        }
    }
}