

using VakilPors.Core.Domain.Dtos;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services;

public interface IThreadCommentService : IScopedDependency
{
    Task<ThreadCommentDto> CreateComment(int userId, ThreadCommentDto commentDto);
    Task<ThreadCommentDto> UpdateComment(int userId, ThreadCommentDto commentDto);
    Task<bool> DeleteComment(int userId, int commentId);
    Task<List<ThreadCommentDto>> GetCommentsForThread(int threadId);
    Task<ThreadCommentDto> GetCommentById(int commentId);
}

