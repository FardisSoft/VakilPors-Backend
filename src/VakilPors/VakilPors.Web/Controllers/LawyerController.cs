using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination.EntityFrameworkCore.Extensions;
using VakilPors.Api.Controllers;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Response;
using VakilPors.Core.Mapper;
using X.PagedList;
using VakilPors.Core.Domain.Dtos.Search;

namespace VakilPors.Web.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class LawyerController : MyControllerBase
{
    private readonly ILawyerServices _lawyerServices;
    private readonly IMapper _mapper;
    private readonly ILogger<LawyerController> _logger;

    public LawyerController(ILawyerServices _lawyerServices, IMapper mapper, ILogger<LawyerController> logger)
    {
        this._mapper = mapper;
        _logger = logger;
        this._lawyerServices = _lawyerServices;
    }

    [HttpPut]
    public async Task<IActionResult> UpdateLawyer([FromForm] LawyerDto lawyerDto)
    {
        _logger.LogInformation($"update lawyer attempt for {lawyerDto.User.UserName ?? lawyerDto.Id.ToString()}");
        var result = await _lawyerServices.UpdateLawyer(lawyerDto);
        return Ok(new AppResponse<object>(result, "Profile Updated"));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation($"get all lawyers");
        var result = await _lawyerServices.GetAllLawyers();
        return Ok(new AppResponse<object>(result, "success"));
    }

    [HttpGet]
    public async Task<IActionResult> GetLawyerById(int lawyerId)
    {
        _logger.LogInformation($"get lawyer data by id for {lawyerId}");
        var result = await _lawyerServices.GetLawyerById(lawyerId);
        return Ok(new AppResponse<object>(result, "success"));
    }
    [HttpGet]
    public async Task<IActionResult> GetLawyerByUserId(int userId)
    {
        _logger.LogInformation($"get lawyer data by user id for {userId}");
        var result = await _lawyerServices.GetLawyerByUserId(userId);
        return Ok(new AppResponse<object>(result, "success"));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetCurrentLawyer()
    {
        _logger.LogInformation($"get current lawyer data");
        var result = await _lawyerServices.GetLawyerByUserId(getUserId());
        return Ok(new AppResponse<object>(result, "success"));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> VerifyLawyer(int lawyerId)
    {
        _logger.LogInformation($"verify lawyer");
        var result = await _lawyerServices.VerifyLawyer(lawyerId);
        return Ok(new AppResponse<object>(result, "success"));
    }


    [HttpGet]
    [Authorize]
    public async Task<IActionResult> TransferToken()
    {
        _logger.LogInformation($"transfer tokens");
        var result = await _lawyerServices.TransferToken(getUserId());
        return Ok(new AppResponse<object>(result, "success"));
    }

    [HttpGet]
    public async Task<ActionResult> GetAllPaged([FromQuery] PagedParams pagedParams, [FromQuery] FilterParams filterParams , [FromQuery] SearchDto searchdto)
    {
        _logger.LogInformation($"GET ALL lawyers paged. page no:{pagedParams.PageNumber} page size:{pagedParams.PageSize}, search query:{filterParams.Q}, sort by:{filterParams.Sort}, isAscending:{filterParams.IsAscending}");
        var all = await _lawyerServices.GetLawyers(pagedParams, filterParams,searchdto);
        var res = all.ToMappedPagination<Lawyer, LawyerDto>(_mapper,pagedParams.PageSize);
        return Ok(new AppResponse<Pagination<LawyerDto>>(res, "success"));
    }

    //[HttpGet]
    //public async Task<ActionResult<List<LawyerDto>>> FilteredSearch([FromQuery] SearchDto searchdto)
    //{
    //    _logger.LogInformation($"filtered search");
    //    var res = await _lawyerServices.FilteredSearch(searchdto);
    //    return Ok(res);
    //}


}