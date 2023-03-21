
using Microsoft.EntityFrameworkCore.Infrastructure;
using VakilPors.Contracts.Repositories;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Domain.Entities;
using VakilPors.Data.Context;

namespace VakilPors.Data.UnitOfWork;

public class AppUnitOfWork : IAppUnitOfWork
{
    private readonly AppDbContext _dbContext;

    public AppUnitOfWork(AppDbContext dbContext)
        => _dbContext = dbContext;

    private DatabaseFacade _database => _dbContext.Database;

    public async Task<int> SaveChangesAsync()
        => await _dbContext.SaveChangesAsync();


    #region Repos

    public IGenericRepo<User> UserRepo => _dbContext.GetService<IGenericRepo<User>>();

    #endregion

}