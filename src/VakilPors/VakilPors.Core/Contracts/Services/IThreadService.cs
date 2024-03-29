﻿
using Pagination.EntityFrameworkCore.Extensions;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services;

public interface IThreadService : IScopedDependency
{
    Task<ThreadDto> CreateThread(int userId, ThreadDto threadDto,IAntiSpam antispam = null );
    Task<ThreadDto> UpdateThread(int userId, ThreadDto threadDto,IAntiSpam antispam = null);
    Task<bool> DeleteThread(int userId, int threadId);
    Task<List<ThreadDto>> GetThreadList(int userId);
    Task<ThreadWithCommentsDto> GetThreadWithComments(int userId, int threadId,PagedParams pagedParams);

    Task<int> LikeThread(int userId, int threadId);

    Task<int> UndoLikeThread(int userId, int threadId);
    Task<Pagination<ForumThread>> SearchThread(string title, PagedParams pagedParams, SortParams sortParam, int userid);
}

