using System.ComponentModel;
using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;

namespace VakilPors.Core.Services;

public class ThreadService : IThreadService
{
    private readonly IAppUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IThreadCommentService _threadCommentService;
    private readonly ILawyerServices _lawyerServices;
    private readonly IPremiumService _premiumService;

    public ThreadService(IAppUnitOfWork uow, IMapper mapper, IThreadCommentService threadCommentService, ILawyerServices lawyerServices, IPremiumService premiumService)
    {
        _uow = uow;
        _mapper = mapper;
        _threadCommentService = threadCommentService;
        _lawyerServices = lawyerServices;
        _premiumService = premiumService;
    }


    public async Task<ThreadDto> CreateThread(int userId, ThreadDto threadDto)
    {
        var anti_spam = new AntiSpamService();
        var result = await anti_spam.IsSpam(threadDto.Description);
        if(result == "This message is detected as a spam and can not be shown.")
        {
            throw new BadArgumentException(result);
        }
        var thread = new ForumThread()
        {
            UserId = userId,
            Title = threadDto.Title,
            Description = threadDto.Description,
            HasAnswer = false,
            CreateDate = DateTime.Now,
            LikeCount = 0
        };
        
        await _uow.ForumThreadRepo.AddAsync(thread);

        var addResult = await _uow.SaveChangesAsync();
        if (addResult <= 0)
            throw new Exception();

        return (await GetThreadWithComments(userId, thread.Id)).Thread;
    }

    public async Task<ThreadDto> UpdateThread(int userId, ThreadDto threadDto)
    {
        var anti_spam = new AntiSpamService();
        var result = await anti_spam.IsSpam(threadDto.Description);
        if (result == "This message is detected as a spam and can not be shown.")
        {
            throw new BadArgumentException(result);
        }

        var foundThread = await _uow.ForumThreadRepo.FindAsync(threadDto.Id);

        if (foundThread == null)
            throw new BadArgumentException("thread not found");

        if (foundThread.UserId != userId)
            throw new AccessViolationException("You do not have permission to perform this action");

        foundThread.Title = threadDto.Title;
        foundThread.Description = threadDto.Description;

        _uow.ForumThreadRepo.Update(foundThread);

        var updateResult = await _uow.SaveChangesAsync();
        if (updateResult <= 0)
            throw new Exception();

        return (await GetThreadWithComments(userId, foundThread.Id)).Thread;
    }

    public async Task<bool> DeleteThread(int userId, int threadId)
    {
        var foundThread = await _uow.ForumThreadRepo.FindAsync(threadId);

        if (foundThread == null)
            throw new BadArgumentException("thread not found");

        if (foundThread.UserId != userId)
            throw new AccessViolationException("You do not have permission to perform this action");

        _uow.ForumThreadRepo.Remove(foundThread);

        var removeResult = await _uow.SaveChangesAsync();
        if (removeResult <= 0)
            throw new Exception();

        return true;
    }

    public async Task<List<ThreadDto>> GetThreadList(int userId)
    {
        var threads = await _uow.ForumThreadRepo
            .AsQueryable()
            .Include(x => x.User)
            .ToListAsync();

        var threadDtos = new List<ThreadDto>();

        foreach (var thread in threads)
        {
            threadDtos.Add(await GetThreadDtoFromThread(userId, thread));
        }
                
        return threadDtos;
    } 

    public async Task<ThreadWithCommentsDto> GetThreadWithComments(int userId, int threadId)
    {
        var thread = await _uow.ForumThreadRepo
            .AsQueryable()
            .Include(x => x.User)
            .Where(x => x.Id == threadId)
            .FirstOrDefaultAsync();

        if (thread == null)
            throw new BadArgumentException("thread not found");

        var threadDto = await GetThreadDtoFromThread(userId, thread);
        
        return new ThreadWithCommentsDto
        {
            Thread = threadDto,
            Comments = await _threadCommentService.GetCommentsForThread(userId, threadId)
        };
    }

    public async Task<int> LikeThread(int userId, int threadId)
    {
        var foundThread = await _uow.ForumThreadRepo
            .AsQueryable()
            .Include(x => x.UserLikes)
            .FirstOrDefaultAsync(x => x.Id == threadId);

        if (foundThread == null)
            throw new BadArgumentException("thread not found");

        var like = foundThread.UserLikes.FirstOrDefault(x => x.UserId == userId);
        if (like != null)
            return foundThread.LikeCount;

        foundThread.LikeCount++;
        foundThread.UserLikes.Add(new UserThreadLike{ThreadId = threadId, UserId = userId});

        _uow.ForumThreadRepo.Update(foundThread);

        var updateResult = await _uow.SaveChangesAsync();
        if (updateResult <= 0)
            throw new Exception();

        return foundThread.LikeCount;
    }

    public async Task<int> UndoLikeThread(int userId, int threadId)
    {
        var foundThread = await _uow.ForumThreadRepo
            .AsQueryable()
            .Include(x => x.UserLikes)
            .FirstOrDefaultAsync(x => x.Id == threadId);

        if (foundThread == null)
            throw new BadArgumentException("thread not found");

        var like = foundThread.UserLikes.FirstOrDefault(x => x.ThreadId == threadId && x.UserId == userId);

        if (foundThread.LikeCount >= 1 && like != null)
        {
            foundThread.UserLikes.Remove(like);
            foundThread.LikeCount--;

            _uow.ForumThreadRepo.Update(foundThread);

            var updateResult = await _uow.SaveChangesAsync();
            if (updateResult <= 0)
                throw new Exception();
        }

        return foundThread.LikeCount;
    }

    private async Task<ThreadDto> GetThreadDtoFromThread(int userId, ForumThread thread)
    {
        var threadDto = new ThreadDto()
        {
            Id = thread.Id,
            Title = thread.Title,
            CommentCount = await _threadCommentService.GetCommentCountForThread(thread.Id),
            CreateDate = thread.CreateDate,
            Description = thread.Description,
            UserId = thread.UserId,
            LikeCount = thread.LikeCount, 
            AnswerCount = await _threadCommentService.GetThreadAnswerCount(thread.Id),
            HasAnswer = await _threadCommentService.IsThreadHasAnswer(thread.Id),
            IsCurrentUserLikedThread = await IsThreadLikedByUser(userId, thread.Id),
            User = new ForumUserDto()
            {
                UserId = thread.UserId,
                Name = thread.User.Name,
                IsLawyer = await _lawyerServices.IsLawyer(thread.UserId),
                IsPremium = await _premiumService.DoseUserHaveAnyActiveSubscription(thread.UserId)
            }
        };

        return threadDto;
    }

    private async Task<bool> IsThreadLikedByUser(int userId, int threadId)
    {
        var likes = await _uow.ForumThreadRepo
            .AsQueryable()
            .Include(x => x.UserLikes)
            .Where(x => x.Id == threadId && x.UserLikes.FirstOrDefault(l => l.UserId == userId) != null)
            .CountAsync();

        return likes > 0;
    }
}

