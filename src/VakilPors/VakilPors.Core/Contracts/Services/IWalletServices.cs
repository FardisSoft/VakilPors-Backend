using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pagination.EntityFrameworkCore.Extensions;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Services;
using X.PagedList;

namespace VakilPors.Core.Contracts.Services
{
    public interface IWalletServices : IScopedDependency
    {
        Task AddBalance(string phoneNumber, decimal amount);
        Task AddBalance(int userID, decimal amount);
        Task<decimal> GetBalance(string phoneNumber);
        Task AddTransaction(int userId, decimal amount, string description, string authority, bool isSuccess, bool isIncome, bool isWithdraw = false);
        Task ApproveTransaction(int transactionId);
        Task ApplyTransaction(int transactionId);
        Task<Pagination<Transaction>> GetTransactions(string phoneNumber, PagedParams pagedParams);
        public Task<IEnumerable<Transaction>> GetWithdrawTransactions();
        Task Withdraw(int userId, decimal amount, string cardNo);
        Task PayWithdraw(int transactionId);
        // Task<bool> HasEnoughBalance(string phoneNumber, decimal amount);
        // Task<bool> HasEnoughBalance(string phoneNumber, decimal amount, decimal discount);
    }
}