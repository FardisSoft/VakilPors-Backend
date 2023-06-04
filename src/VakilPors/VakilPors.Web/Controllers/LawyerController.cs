using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Api.Controllers;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Response;
using VakilPors.Core.Mapper;
using X.PagedList;

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
    public async Task<IActionResult> TransferToken(int lawyerId)
    {
        _logger.LogInformation($"transfer tokens");
        var result = await _lawyerServices.TransferToken(lawyerId);
        return Ok(new AppResponse<object>(result, "success"));
    }

    [HttpGet]
    public async Task<ActionResult<IPagedList<LawyerDto>>> GetAllPaged([FromQuery] PagedParams pagedParams, [FromQuery] FilterParams filterParams)
    {
        _logger.LogInformation($"GET ALL lawyers paged. page no:{pagedParams.PageNumber} page size:{pagedParams.PageSize}, search query:{filterParams.Q}, sort by:{filterParams.Sort}, isAscending:{filterParams.IsAscending}");
        var all = await _lawyerServices.GetLawyers(pagedParams, filterParams);
        var res = all.ToMappedPagedList<Lawyer, LawyerDto>(_mapper);
        return Ok(new AppResponse<IPagedList<LawyerDto>>(res, "success"));
    }

}