

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
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
    private readonly ILawyerServices _lawyerServices;

    public ThreadCommentService(IAppUnitOfWork uow, IMapper mapper, ILawyerServices lawyerServices)
    {
        _uow = uow;
        _mapper = mapper;
        _lawyerServices = lawyerServices;
    }


    public async Task<ThreadCommentDto> CreateComment(int userId, ThreadCommentDto commentDto)
    {
        var comment = new ThreadComment()
        {
            UserId = userId,
            ThreadId = commentDto.ThreadId,
            Text = commentDto.Text,
            CreateDate = DateTime.Now,
            LikeCount = 0,
            IsSetAsAnswer = false
        };

        await _uow.ThreadCommentRepo.AddAsync(comment);

        var addResult = await _uow.SaveChangesAsync();
        if (addResult <= 0)
            throw new Exception();

        return await GetCommentById(comment.Id);
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

        return await GetCommentDtoFromComment(foundComment);
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
    {
        var comments = await _uow.ThreadCommentRepo
            .AsQueryable()
            .Where(x => x.ThreadId == threadId)
            .Include(x => x.User)
            .ToListAsync();

        var commentDtoList = new List<ThreadCommentDto>();

        foreach (var comment in comments)
        {
            commentDtoList.Add(await GetCommentDtoFromComment(comment));
        }

        return commentDtoList;
    }
        
    

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

        return await GetCommentDtoFromComment(comment);

    }

    public async Task<int> GetCommentCountForThread(int threadId)
        => await _uow.ThreadCommentRepo
            .AsQueryable()
            .Where(x => x.ThreadId == threadId)
            .CountAsync();

    public async Task<int> LikeComment(int commentId)
    {
        var foundComment = await _uow.ThreadCommentRepo.FindAsync(commentId);

        if (foundComment == null)
            throw new BadArgumentException("comment not found");

        foundComment.LikeCount++;

        _uow.ThreadCommentRepo.Update(foundComment);

        var updateResult = await _uow.SaveChangesAsync();
        if (updateResult <= 0)
            throw new Exception();

        return foundComment.LikeCount;
    }

    public async Task<int> UndoLikeComment(int commentId)
    {
        var foundComment = await _uow.ThreadCommentRepo.FindAsync(commentId);

        if (foundComment == null)
            throw new BadArgumentException("comment not found");

        if (foundComment.LikeCount >= 1)
        {
            foundComment.LikeCount--;

            _uow.ThreadCommentRepo.Update(foundComment);

            var updateResult = await _uow.SaveChangesAsync();
            if (updateResult <= 0)
                throw new Exception();
        }

        return foundComment.LikeCount;
    }

    public async Task<bool> SetAsAnswer(int userId, int commentId)
    {
        var foundComment = await _uow.ThreadCommentRepo
            .AsQueryable()
            .Include(x => x.Thread)
            .FirstOrDefaultAsync(x => x.Id == commentId);

        if (foundComment == null)
            throw new BadArgumentException("comment not found");

        if (foundComment.Thread.UserId != userId)
            throw new AccessViolationException("You do not have permission to perform this action");

        foundComment.IsSetAsAnswer = true;

        _uow.ThreadCommentRepo.Update(foundComment);

        var updateResult = await _uow.SaveChangesAsync();
        if (updateResult <= 0)
            throw new Exception();

        return true;
    }

    public async Task<bool> UndoSetAsAnswer(int userId, int commentId)
    {
        var foundComment = await _uow.ThreadCommentRepo
            .AsQueryable()
            .Include(x => x.Thread)
            .FirstOrDefaultAsync(x => x.Id == commentId);

        if (foundComment == null)
            throw new BadArgumentException("comment not found");

        if (foundComment.Thread.UserId != userId)
            throw new AccessViolationException("You do not have permission to perform this action");

        foundComment.IsSetAsAnswer = false;

        _uow.ThreadCommentRepo.Update(foundComment);

        var updateResult = await _uow.SaveChangesAsync();
        if (updateResult <= 0)
            throw new Exception();

        return true;
    }

    public async Task<bool> IsThreadHasAnswer(int threadId)
    {
        var comments = await _uow.ThreadCommentRepo
            .AsQueryable()
            .Where(x => x.ThreadId == threadId && x.IsSetAsAnswer == true)
            .CountAsync();

        return comments > 0;
    }
    private async Task<ThreadCommentDto> GetCommentDtoFromComment(ThreadComment comment)
    {
        var threadCommentDto = new ThreadCommentDto()
        {
            Id = comment.Id,
            CreateDate = comment.CreateDate,
            IsSetAsAnswer = comment.IsSetAsAnswer,
            Text = comment.Text,
            LikeCount = comment.LikeCount,
            UserId = comment.UserId,
            User = new ForumUserDto()
            {
                UserId = comment.UserId,
                Name = comment.User.Name,
                IsLawyer = await _lawyerServices.IsLawyer(comment.UserId),
                IsPremium = false
            }
        };

        return threadCommentDto;
    }
}

