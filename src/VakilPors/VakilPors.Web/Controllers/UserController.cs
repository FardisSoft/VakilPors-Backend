using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination.EntityFrameworkCore.Extensions;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.Search;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Mapper;
using VakilPors.Shared.Response;
using VakilPors.Web.Controllers;

namespace VakilPors.Api.Controllers;
[Authorize(Roles = RoleNames.Admin)]
[ApiController]
[Route("[controller]/[action]")]
public class UserController:MyControllerBase
{
    private readonly IUserServices _userServices;
    private readonly IMapper _mapper;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserServices userServices,IMapper mapper, ILogger<UserController> logger)
    {
        _userServices = userServices;
        _mapper = mapper;
        _logger = logger;
    }
    [HttpGet]
    public async Task<ActionResult> GetAllPaged([FromQuery]string? query,[FromQuery]int? roleId,[FromQuery] PagedParams pagedParams, [FromQuery] SortParams sortParams)
    {
        _logger.LogInformation($"GET ALL users paged by admin with phone number:{getPhoneNumber()}. page no:{pagedParams.PageNumber} page size:{pagedParams.PageSize}, search query:{query}, sort by:{sortParams.Sort}, isAscending:{sortParams.IsAscending}");
        var all = await _userServices.GetUsers(query,roleId,pagedParams, sortParams);
        // var res = all.ToMappedPagination<Us, UserDto>(_mapper,pagedParams.PageSize);
        return Ok(new AppResponse<Pagination<UserDto>>(all, "success"));
    }
}