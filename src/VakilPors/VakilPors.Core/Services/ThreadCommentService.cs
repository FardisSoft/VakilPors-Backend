

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Threading;
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
    private readonly IPremiumService _premiumService;

    public ThreadCommentService(IAppUnitOfWork uow, IMapper mapper, ILawyerServices lawyerServices, IPremiumService premiumService)
    {
        _uow = uow;
        _mapper = mapper;
        _lawyerServices = lawyerServices;
        _premiumService = premiumService;
    }


    public async Task<ThreadCommentDto> CreateComment(int userId, ThreadCommentDto commentDto)
    {
        var check_2minutes = await CheckWithin2Minutes(userId, commentDto);
        var anti_spam = new AntiSpamService();
        var result = await anti_spam.IsSpam(commentDto.Text);
        if (result == "This message is detected as a spam and can not be shown.")
        {
            throw new BadArgumentException(result);
        }
        if (check_2minutes == "wrong")
        {
            throw new BadArgumentException("The new comment should be sent within 2 minutes after the last comment");
        }
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

        var lawyer = await _uow.LawyerRepo.AsQueryable().FirstOrDefaultAsync(x => x.UserId == userId);
        if (lawyer != null)
            await _lawyerServices.AddToken(lawyer.Id, 1);

        return await GetCommentById(userId, comment.Id);
    }

    public async Task<string> CheckWithin2Minutes(int userId, ThreadCommentDto commentDto)
    {
        var newest_comment = await _uow.ThreadCommentRepo.AsQueryable().Where(x => x.UserId == userId).OrderByDescending(x =>x.CreateDate).FirstOrDefaultAsync();
        if (newest_comment == null)
        {
            return "ok";
        }
        var date = newest_comment.CreateDate;
        DateTime now = DateTime.Now;
        TimeSpan timeSinceComment = now - date;

        if (timeSinceComment.TotalMinutes > 2 )
        {
            return "ok";
        }
        else
        {
            return "wrong";
        }
    }

    public async Task<ThreadCommentDto> UpdateComment(int userId, ThreadCommentDto commentDto)
    {
        var anti_spam = new AntiSpamService();
        var result = await anti_spam.IsSpam(commentDto.Text);
        if (result == "This message is detected as a spam and can not be shown.")
        {
            throw new BadArgumentException(result);
        }
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

        return await GetCommentById(userId, foundComment.Id);
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

    public async Task<List<ThreadCommentDto>> GetCommentsForThread(int userId, int threadId)
    {
        var comments = await _uow.ThreadCommentRepo
            .AsQueryable()
            .Where(x => x.ThreadId == threadId)
            .Include(x => x.User)
            .ToListAsync();

        var commentDtoList = new List<ThreadCommentDto>();

        foreach (var comment in comments)
        {
            commentDtoList.Add(await GetCommentDtoFromComment(userId, comment));
        }

        commentDtoList = commentDtoList
            .OrderByDescending(x => x.User.IsPremium)
            .ThenByDescending(x => x.LikeCount)
            .ToList();

        return commentDtoList;
    }
    
    public async Task<ThreadCommentDto> GetCommentById(int userId, int commentId)
    {
        var comment = await _uow.ThreadCommentRepo
            .AsQueryable()
            .Include(x => x.User)
            .Include(x => x.Thread)
            .Where(x => x.Id == commentId)
            .FirstOrDefaultAsync();

        if (comment == null)
            throw new BadArgumentException("comment not found");

        return await GetCommentDtoFromComment(userId, comment);

    }

    public async Task<int> GetCommentCountForThread(int threadId)
        => await _uow.ThreadCommentRepo
            .AsQueryable()
            .Where(x => x.ThreadId == threadId)
            .CountAsync();

    public async Task<int> LikeComment(int userId, int commentId)
    {
        var foundComment = await _uow.ThreadCommentRepo
            .AsQueryable()
            .Include(x => x.UserLikes)
            .FirstOrDefaultAsync(x => x.Id == commentId);

        if (foundComment == null)
            throw new BadArgumentException("comment not found");

        var like = foundComment.UserLikes.FirstOrDefault(x => x.UserId == userId);
        if (like != null)
            return foundComment.LikeCount;

        foundComment.UserLikes.Add(new UserCommentLike { CommentId = commentId, UserId = userId });
        foundComment.LikeCount++;

        _uow.ThreadCommentRepo.Update(foundComment);

        var updateResult = await _uow.SaveChangesAsync();
        if (updateResult <= 0)
            throw new Exception();

        return foundComment.LikeCount;
    }

    public async Task<int> UndoLikeComment(int userId, int commentId)
    {
        var foundComment = await _uow.ThreadCommentRepo
            .AsQueryable()
            .Include(x => x.UserLikes)
            .FirstOrDefaultAsync(x => x.Id == commentId);

        if (foundComment == null)
            throw new BadArgumentException("comment not found");

        var like = foundComment.UserLikes.FirstOrDefault(x => x.UserId == userId);
        if (foundComment.LikeCount >= 1 && like != null)
        {
            foundComment.LikeCount--;
            foundComment.UserLikes.Remove(like);

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

        var lawyer = await _uow.LawyerRepo.AsQueryable().FirstOrDefaultAsync(x => x.UserId == foundComment.UserId);
        if (lawyer != null)
            await _lawyerServices.AddToken(lawyer.Id, 5);

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

    public async Task<int> GetThreadAnswerCount(int threadId)
    {
        var comments = await _uow.ThreadCommentRepo
            .AsQueryable()
            .Where(x => x.ThreadId == threadId && x.IsSetAsAnswer == true)
            .CountAsync();

        return comments;
    }
    private async Task<ThreadCommentDto> GetCommentDtoFromComment(int userId, ThreadComment comment)
    {
        var threadCommentDto = new ThreadCommentDto()
        {
            Id = comment.Id,
            CreateDate = comment.CreateDate,
            IsSetAsAnswer = comment.IsSetAsAnswer,
            Text = comment.Text,
            LikeCount = comment.LikeCount,
            UserId = comment.UserId,
            IsCurrentUserLikedComment = await IsCommentLikedByUser(userId, comment.Id),
            User = new ForumUserDto()
            {
                UserId = comment.UserId,
                Name = comment.User.Name,
                IsLawyer = await _lawyerServices.IsLawyer(comment.UserId),
                IsPremium = await _premiumService.DoseUserHaveAnyActiveSubscription(comment.UserId)
            }
        };

        return threadCommentDto;
    }

    private async Task<bool> IsCommentLikedByUser(int userId, int commentId)
    {
        var likes = await _uow.ThreadCommentRepo
            .AsQueryable()
            .Include(x => x.UserLikes)
            .Where(x => x.Id == commentId && x.UserLikes.FirstOrDefault(l => l.UserId == userId && l.CommentId == commentId) != null)
            .CountAsync();

        return likes > 0;
    }
}

