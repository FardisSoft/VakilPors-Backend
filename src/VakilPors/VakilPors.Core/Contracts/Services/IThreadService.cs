
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services;

public interface IThreadService : IScopedDependency
{
    Task<ThreadDto> CreateThread(int userId, ThreadDto threadDto);
    Task<ThreadDto> UpdateThread(int userId, ThreadDto threadDto);
    Task<bool> DeleteThread(int userId, int threadId);
    Task<List<ThreadDto>> GetThreadList(int userId);
    Task<ThreadWithCommentsDto> GetThreadWithComments(int userId, int threadId,PagedParams pagedParams);

    Task<int> LikeThread(int userId, int threadId);

    Task<int> UndoLikeThread(int userId, int threadId);
}

