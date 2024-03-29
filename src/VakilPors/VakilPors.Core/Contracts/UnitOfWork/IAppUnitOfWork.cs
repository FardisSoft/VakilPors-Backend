using Microsoft.AspNetCore.Identity;
using VakilPors.Contracts.Repositories;
using VakilPors.Core.Domain.Entities;
using ForumThread = VakilPors.Core.Domain.Entities.ForumThread;

namespace VakilPors.Contracts.UnitOfWork;

public interface IAppUnitOfWork
{
    public Task<int> SaveChangesAsync();

    #region Repos

    public IGenericRepo<User> UserRepo { get; }
    public IGenericRepo<IdentityUserRole<int>> UserRolesRepo { get; }
    public IGenericRepo<Role> RoleRepo { get; }
    public IGenericRepo<Transaction> TransactionRepo { get; }
    public IGenericRepo<Lawyer> LawyerRepo { get; }
    public IGenericRepo<ForumThread> ForumThreadRepo { get; }
    public IGenericRepo<ThreadComment> ThreadCommentRepo { get; }
    public IGenericRepo<Chat> ChatRepo { get; }
    public IGenericRepo<ChatMessage> ChatMessageRepo { get; }

    public IGenericRepo<Premium> PremiumRepo { get; }
    public IGenericRepo<Subscribed> SubscribedRepo { get; }

    public IGenericRepo<LegalDocument> DocumentRepo { get; }
    public IGenericRepo<DocumentAccess> DocumentAccessRepo { get; }
    public IGenericRepo<Rate> RateRepo { get; }
    public IGenericRepo<Visitor> VisitorRepo { get; }
    public IGenericRepo<Event> EventRepo { get; }
    public IGenericRepo<Report> ReportRepo { get; }

    #endregion
}

