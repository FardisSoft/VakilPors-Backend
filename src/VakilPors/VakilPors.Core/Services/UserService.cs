
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;

namespace VakilPors.Core.Services;

public class UserService : IUserServices
{
    private readonly IAppUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UserService(IAppUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<UserDto> UpdateUser(UserDto userDto)
    {
        var foundUser = await _uow.UserRepo.FindAsync(userDto.Id);
        if (foundUser == null)
            throw new BadArgumentException("user not found");

        foundUser.Name = userDto.Name;
        foundUser.Email = userDto.Email;
        foundUser.Job = userDto.Job;
        foundUser.Bio = userDto.Bio;
        foundUser.ProfileImageUrl = userDto.ProfileImageUrl;

        _uow.UserRepo.Update(foundUser);
        var updateResult = await _uow.SaveChangesAsync();
        if (updateResult <= 0)
            throw new Exception();

        return userDto;
    }

    public async Task<List<UserDto>> GetAllUsers()
        => await _uow.UserRepo
            .AsQueryable()
            .Select(x => _mapper.Map<UserDto>(x))
            .ToListAsync();


    public async Task<UserDto> GetUserById(int userId)
    {
        var user = await _uow.UserRepo
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            throw new BadArgumentException("User Not Found");

        return _mapper.Map<UserDto>(user);
    }
        

}

