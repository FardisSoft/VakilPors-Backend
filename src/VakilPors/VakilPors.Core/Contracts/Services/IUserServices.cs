using Pagination.EntityFrameworkCore.Extensions;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services;

public interface IUserServices : IScopedDependency
{
    Task<UserDto> UpdateUser(UserDto userDto);
    Task<List<UserDto>> GetAllUsers();
    Task<UserDto> GetUserById(int userId);
    Task<Pagination<User>> GetUsers(string query, PagedParams pagedParams, SortParams sortParams);
}

