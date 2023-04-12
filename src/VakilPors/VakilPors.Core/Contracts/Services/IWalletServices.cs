using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        Task AddTransaction(int userId, decimal amount, string description,string authority, bool isSuccess, bool isIncome);
        Task ApproveTransaction(int tranactionId);
        Task ApplyTransaction(int tranactionId);
        Task<IPagedList<Tranaction>> GetTransactions(string phoneNumber,PagedParams pagedParams);
        // Task<bool> HasEnoughBalance(string phoneNumber, decimal amount);
        // Task<bool> HasEnoughBalance(string phoneNumber, decimal amount, decimal discount);
    }
}