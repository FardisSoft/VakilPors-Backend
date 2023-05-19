
using Microsoft.EntityFrameworkCore.Infrastructure;
using VakilPors.Contracts.Repositories;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Domain.Entities;
using VakilPors.Data.Context;
using ForumThread = VakilPors.Core.Domain.Entities.ForumThread;

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

    public IGenericRepo<Tranaction> TransactionRepo => _dbContext.GetService<IGenericRepo<Tranaction>>();

    public IGenericRepo<Lawyer> LawyerRepo => _dbContext.GetService<IGenericRepo<Lawyer>>();

    public IGenericRepo<ForumThread> ForumThreadRepo => _dbContext.GetService<IGenericRepo<ForumThread>>();

    public IGenericRepo<ThreadComment> ThreadCommentRepo => _dbContext.GetService<IGenericRepo<ThreadComment>>();

    public IGenericRepo<Chat> ChatRepo => _dbContext.GetService<IGenericRepo<Chat>>();

    public IGenericRepo<ChatMessage> ChatMessageRepo => _dbContext.GetService<IGenericRepo<ChatMessage>>();

    public IGenericRepo<Premium> PremiumRepo => _dbContext.GetService<IGenericRepo<Premium>>();
    public IGenericRepo<Subscribed> SubscribedRepo => _dbContext.GetService<IGenericRepo<Subscribed>>();

    public IGenericRepo<LegalDocument> DocumentRepo => _dbContext.GetService<IGenericRepo<LegalDocument>>();
    public IGenericRepo<Rate> RateRepo => _dbContext.GetService<IGenericRepo<Rate>>();
    public IGenericRepo<Visitor> VisitorRepo => _dbContext.GetService<IGenericRepo<Visitor>>();

    #endregion

}