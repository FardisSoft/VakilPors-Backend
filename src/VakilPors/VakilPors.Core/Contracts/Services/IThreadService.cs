
using VakilPors.Core.Domain.Dtos;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services;

public interface IThreadService : IScopedDependency
{
    Task<ThreadDto> CreateThread(int userId, ThreadDto threadDto);
    Task<ThreadDto> UpdateThread(int userId, ThreadDto threadDto);
    Task<bool> DeleteThread(int userId, int threadId);
    Task<List<ThreadDto>> GetThreadList();
    Task<ThreadWithCommentsDto> GetThreadWithComments(int threadId);

    Task<int> LikeThread(int threadId);

    Task<int> UndoLikeThread(int threadId);
}

