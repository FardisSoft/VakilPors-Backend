using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Shared.Response;
using VakilPors.Web.Controllers;

namespace VakilPors.Api.Controllers;

public class CustomerController : MyControllerBase
{
    private readonly IUserServices _userServices;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(IUserServices userServices, ILogger<CustomerController> logger)
    {
        _userServices = userServices;
        _logger = logger;
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser(UserDto userDto)
    {
        _logger.LogInformation($"update user attempt for {userDto.UserName}");
        var result = await _userServices.UpdateUser(userDto);
        return Ok(new AppResponse<UserDto>(result, "Profile Updated"));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        _logger.LogInformation($"get all users");
        var result = await _userServices.GetAllUsers();
        return Ok(new AppResponse<object>(result, "success"));
    }

    [HttpGet]
    public async Task<IActionResult> GetUserById(int userId)
    {
        _logger.LogInformation($"get user data by id for {userId}");
        var result = await _userServices.GetUserById(userId);
        return Ok(new AppResponse<object>(result, "success"));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
       => await GetUserById(getUserId());
}

