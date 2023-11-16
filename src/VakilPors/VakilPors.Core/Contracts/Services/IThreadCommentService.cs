

using Pagination.EntityFrameworkCore.Extensions;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services;

public interface IThreadCommentService : IScopedDependency
{
    Task<ThreadCommentDto> CreateComment(int userId, ThreadCommentDto commentDto);
    Task<ThreadCommentDto> UpdateComment(int userId, ThreadCommentDto commentDto);
    Task<bool> DeleteComment(int userId, int commentId);
    Task<Pagination<ThreadCommentDto>> GetCommentsForThread(int userId, int threadId,PagedParams pagedParams);
    Task<ThreadCommentDto> GetCommentById(int userId, int commentId);
    Task<int> GetCommentCountForThread(int threadId);
    Task<int> LikeComment(int userId, int commentId);
    Task<int> UndoLikeComment(int userId, int commentId);
    Task<bool> SetAsAnswer(int userId, int commentId);
    Task<bool> UndoSetAsAnswer(int userId, int commentId);
    Task<bool> IsThreadHasAnswer(int threadId);
    Task<int> GetThreadAnswerCount(int threadId);
}

