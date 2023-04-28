using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

        return (await GetThreadWithComments(thread.Id)).Thread;
    }

    public async Task<ThreadDto> UpdateThread(int userId, ThreadDto threadDto)
    {
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

        return await GetThreadDtoFromThread(foundThread);
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

    public async Task<List<ThreadDto>> GetThreadList()
    {
        var threads = await _uow.ForumThreadRepo
            .AsQueryable()
            .Include(x => x.User)
            .ToListAsync();

        var threadDtos = new List<ThreadDto>();

        foreach (var thread in threads)
        {
            threadDtos.Add(await GetThreadDtoFromThread(thread));
        }
                
        return threadDtos;
    } 

    public async Task<ThreadWithCommentsDto> GetThreadWithComments(int threadId)
    {
        var thread = await _uow.ForumThreadRepo
            .AsQueryable()
            .Include(x => x.User)
            .Where(x => x.Id == threadId)
            .FirstOrDefaultAsync();

        if (thread == null)
            throw new BadArgumentException("thread not found");

        var threadDto = await GetThreadDtoFromThread(thread);
        
        return new ThreadWithCommentsDto
        {
            Thread = threadDto,
            Comments = await _threadCommentService.GetCommentsForThread(threadId)
        };
    }

    public async Task<int> LikeThread(int threadId)
    {
        var foundThread = await _uow.ForumThreadRepo.FindAsync(threadId);

        if (foundThread == null)
            throw new BadArgumentException("thread not found");


        foundThread.LikeCount++;

        _uow.ForumThreadRepo.Update(foundThread);

        var updateResult = await _uow.SaveChangesAsync();
        if (updateResult <= 0)
            throw new Exception();

        return foundThread.LikeCount;
    }

    public async Task<int> UndoLikeThread(int threadId)
    {
        var foundThread = await _uow.ForumThreadRepo.FindAsync(threadId);

        if (foundThread == null)
            throw new BadArgumentException("thread not found");

        if (foundThread.LikeCount >= 1)
        {
            foundThread.LikeCount--;

            _uow.ForumThreadRepo.Update(foundThread);

            var updateResult = await _uow.SaveChangesAsync();
            if (updateResult <= 0)
                throw new Exception();
        }

        return foundThread.LikeCount;
    }

    private async Task<ThreadDto> GetThreadDtoFromThread(ForumThread thread)
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
            HasAnswer = await _threadCommentService.IsThreadHasAnswer(thread.Id),
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
}

