

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;

namespace VakilPors.Core.Services;

public class ThreadCommentService : IThreadCommentService
{
    private readonly IAppUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ThreadCommentService(IAppUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }


    public async Task<ThreadCommentDto> CreateComment(int userId, ThreadCommentDto commentDto)
    {
        var comment = new ThreadComment()
        {
            UserId = userId,
            ThreadId = commentDto.ThreadId,
            Text = commentDto.Text,
        };

        await _uow.ThreadCommentRepo.AddAsync(comment);

        var addResult = await _uow.SaveChangesAsync();
        if (addResult <= 0)
            throw new Exception();

        return _mapper.Map<ThreadCommentDto>(comment);
    }

    public async Task<ThreadCommentDto> UpdateComment(int userId, ThreadCommentDto commentDto)
    {
        var foundComment = await _uow.ThreadCommentRepo.FindAsync(commentDto.Id);

        if (foundComment == null)
            throw new BadArgumentException("comment not found");

        if (foundComment.UserId != userId)
            throw new AccessViolationException("You do not have permission to perform this action");

        foundComment.Text = commentDto.Text;

        _uow.ThreadCommentRepo.Update(foundComment);

        var updateResult = await _uow.SaveChangesAsync();
        if (updateResult <= 0)
            throw new Exception();

        return commentDto;
    }

    public async Task<bool> DeleteComment(int userId, int commentId)
    {
        var foundComment = await _uow.ThreadCommentRepo.FindAsync(commentId);

        if (foundComment == null)
            throw new BadArgumentException("comment not found");

        if (foundComment.UserId != userId)
            throw new AccessViolationException("You do not have permission to perform this action");

        _uow.ThreadCommentRepo.Remove(foundComment);

        var removeResult = await _uow.SaveChangesAsync();
        if (removeResult <= 0)
            throw new Exception();

        return true;
    }

    public async Task<List<ThreadCommentDto>> GetCommentsForThread(int threadId)
        => await _uow.ThreadCommentRepo
            .AsQueryable()
            .Where(x => x.ThreadId == threadId)
            .Include(x => x.User)
            .Select(x => _mapper.Map<ThreadCommentDto>(x))
            .ToListAsync();
    

    public async Task<ThreadCommentDto> GetCommentById(int commentId)
    {
        var comment = await _uow.ThreadCommentRepo
            .AsQueryable()
            .Include(x => x.User)
            .Include(x => x.Thread)
            .Where(x => x.Id == commentId)
            .FirstOrDefaultAsync();

        if (comment == null)
            throw new BadArgumentException("comment not found");

        return _mapper.Map<ThreadCommentDto>(comment);

    }
}

