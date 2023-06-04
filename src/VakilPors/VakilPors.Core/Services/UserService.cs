
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Org.BouncyCastle.Asn1.IsisMtt.X509;
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
    private readonly IAwsFileService _fileService;
    private readonly IPremiumService _premiumService;

    public UserService(IAppUnitOfWork uow, IMapper mapper, IAwsFileService fileService, IPremiumService premiumService)
    {
        _uow = uow;
        _mapper = mapper;
        _fileService = fileService;
        _premiumService = premiumService;
    }

    public async Task<UserDto> UpdateUser(UserDto userDto)
    {
        var foundUser = await _uow.UserRepo.FindAsync(userDto.Id);
        if (foundUser == null)
            throw new BadArgumentException("user not found");

        if (userDto.ProfileImage is { Length: > 0 })
        {
            var profileImageKey = await _fileService.UploadAsync(userDto.ProfileImage);
            if (profileImageKey != null)
                foundUser.ProfileImageUrl = profileImageKey;
        }

        foundUser.Name = userDto.Name;
        foundUser.Email = userDto.Email;
        foundUser.Job = userDto.Job;
        foundUser.Bio = userDto.Bio;

        _uow.UserRepo.Update(foundUser);
        var updateResult = await _uow.SaveChangesAsync();
        if (updateResult <= 0)
            throw new Exception();

        return await GetUserDtoFromUser(foundUser);
    }

    public async Task<List<UserDto>> GetAllUsers()
    {
        var users = await _uow.UserRepo
            .AsQueryable()
            .ToListAsync();

        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            userDtos.Add(await GetUserDtoFromUser(user));
        }

        return userDtos;
    }
    
    public async Task<UserDto> GetUserById(int userId)
    {
        var user = await _uow.UserRepo
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            throw new BadArgumentException("User Not Found");

        return await GetUserDtoFromUser(user);
    }

    private async Task<UserDto> GetUserDtoFromUser(User user)
    {
        var userDto = _mapper.Map<UserDto>(user);

        userDto.IsPremium = await _premiumService.DoseUserHaveAnyActiveSubscription(user.Id);

        return userDto;
    }

    //private UserDto ReplaceImageKeyWithUrl(UserDto userDto)
    //{
    //    if (userDto.ProfileImageUrl != null)
    //        userDto.ProfileImageUrl = _fileService.GetFileUrl(userDto.ProfileImageUrl);

    //    return userDto;
    //}
}

