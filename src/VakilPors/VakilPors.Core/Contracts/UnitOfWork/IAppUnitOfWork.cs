using VakilPors.Contracts.Repositories;
using VakilPors.Core.Domain.Entities;

namespace VakilPors.Contracts.UnitOfWork;

public interface IAppUnitOfWork
{
    public Task<int> SaveChangesAsync();
    
    #region Repos

    public IGenericRepo<User> UserRepo { get; }
    public IGenericRepo<Tranaction> TransactionRepo { get; }
    
    #endregion
}

