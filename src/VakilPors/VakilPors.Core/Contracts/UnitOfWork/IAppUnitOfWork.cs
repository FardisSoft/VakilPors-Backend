using VakilPors.Contracts.Repositories;
using VakilPors.Core.Domain.Entities;
using Thread = VakilPors.Core.Domain.Entities.Thread;

namespace VakilPors.Contracts.UnitOfWork;

public interface IAppUnitOfWork
{
    public Task<int> SaveChangesAsync();
    
    #region Repos

    public IGenericRepo<User> UserRepo { get; }
    public IGenericRepo<Tranaction> TransactionRepo { get; }
    public IGenericRepo<Lawyer> LawyerRepo { get; }
    public IGenericRepo<Thread> ThreadRepo { get; }
    public IGenericRepo<ThreadComment> ThreadCommentRepo { get; }

    #endregion
}

