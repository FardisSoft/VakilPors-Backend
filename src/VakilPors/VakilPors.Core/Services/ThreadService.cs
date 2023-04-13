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

    public ThreadService(IAppUnitOfWork uow, IMapper mapper, IThreadCommentService threadCommentService)
    {
        _uow = uow;
        _mapper = mapper;
        _threadCommentService = threadCommentService;
    }


    public async Task<ThreadDto> CreateThread(int userId, ThreadDto threadDto)
    {
        var thread = new ForumThread()
        {
            UserId = userId,
            Title = threadDto.Title,
            Description = threadDto.Description
        };
        
        await _uow.ForumThreadRepo.AddAsync(thread);

        var addResult = await _uow.SaveChangesAsync();
        if (addResult <= 0)
            throw new Exception();

        return _mapper.Map<ThreadDto>(thread);
    }

    public async Task<ThreadDto> UpdateThread(int userId, ThreadDto threadDto)
    {
        var foundThread = await _uow.ForumThreadRepo.FindAsync(threadDto.Id);

        if (foundThread == null)
            throw new BadArgumentException("comment not found");

        if (foundThread.UserId != userId)
            throw new AccessViolationException("You do not have permission to perform this action");

        foundThread.Title = threadDto.Title;
        foundThread.Description = threadDto.Description;

        _uow.ForumThreadRepo.Update(foundThread);

        var updateResult = await _uow.SaveChangesAsync();
        if (updateResult <= 0)
            throw new Exception();

        return threadDto;
    }

    public async Task<bool> DeleteThread(int userId, int threadId)
    {
        var foundThread = await _uow.ForumThreadRepo.FindAsync(threadId);

        if (foundThread == null)
            throw new BadArgumentException("comment not found");

        if (foundThread.UserId != userId)
            throw new AccessViolationException("You do not have permission to perform this action");

        _uow.ForumThreadRepo.Remove(foundThread);

        var removeResult = await _uow.SaveChangesAsync();
        if (removeResult <= 0)
            throw new Exception();

        return true;
    }

    public async Task<List<ThreadDto>> GetThreadList()
        => await _uow.ForumThreadRepo
            .AsQueryable()
            .Include(x => x.User)
            .Select(x => _mapper.Map<ThreadDto>(x))
            .ToListAsync(); 

    public async Task<ThreadWithCommentsDto> GetThreadWithComments(int threadId)
    {
        var thread = await _uow.ForumThreadRepo
            .AsQueryable()
            .Include(x => x.User)
            .Where(x => x.Id == threadId)
            .FirstOrDefaultAsync();

        if (thread == null)
            throw new BadArgumentException("thread not found");

        return new ThreadWithCommentsDto
        {
            Thread = _mapper.Map<ThreadDto>(thread),
            Comments = await _threadCommentService.GetCommentsForThread(threadId)
        };
    }
}

