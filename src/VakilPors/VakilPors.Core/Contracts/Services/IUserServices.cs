using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services;

public interface IUserServices : IScopedDependency
{
    Task<UserDto> UpdateUser(UserDto userDto);
    Task<List<UserDto>> GetAllUsers();
    Task<UserDto> GetUserById(int userId);
}

