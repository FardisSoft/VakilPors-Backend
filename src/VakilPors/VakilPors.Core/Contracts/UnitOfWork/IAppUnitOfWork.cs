using VakilPors.Contracts.Repositories;
using VakilPors.Core.Domain.Entities;
using ForumThread = VakilPors.Core.Domain.Entities.ForumThread;

namespace VakilPors.Contracts.UnitOfWork;

public interface IAppUnitOfWork
{
    public Task<int> SaveChangesAsync();

    #region Repos

    public IGenericRepo<User> UserRepo { get; }
    public IGenericRepo<Tranaction> TransactionRepo { get; }
    public IGenericRepo<Lawyer> LawyerRepo { get; }
    public IGenericRepo<ForumThread> ForumThreadRepo { get; }
    public IGenericRepo<ThreadComment> ThreadCommentRepo { get; }
    public IGenericRepo<Chat> ChatRepo { get; }
    public IGenericRepo<ChatMessage> ChatMessageRepo { get; }

    public IGenericRepo<Premium> PremiumRepo { get; }
    public IGenericRepo<Subscribed> SubscribedRepo { get; }

    public IGenericRepo<LegalDocument> DocumentRepo { get; }
    public IGenericRepo<Rate> RateRepo { get; }
    public IGenericRepo<Visitor> VisitorRepo { get; }

    #endregion
}

