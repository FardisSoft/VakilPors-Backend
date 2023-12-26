using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination.EntityFrameworkCore.Extensions;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.Search;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Mapper;
using VakilPors.Shared.Response;

namespace VakilPors.Api.Controllers;

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
        var result = await _lawyerServices.GetLawyerByUserId(GetUserId());
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
        var result = await _lawyerServices.TransferToken(GetUserId());
        return Ok(new AppResponse<object>(result, "success"));
    }

    [HttpGet]
    public async Task<ActionResult> GetAllPaged([FromQuery] PagedParams pagedParams, [FromQuery] SortParams sortParams , [FromQuery] LawyerFilterParams filterParams)
    {
        _logger.LogInformation($"GET ALL lawyers paged. page no:{pagedParams.PageNumber} page size:{pagedParams.PageSize}, search query:{filterParams.Name}, sort by:{sortParams.Sort}, isAscending:{sortParams.IsAscending}");
        var all = await _lawyerServices.GetLawyers(pagedParams, sortParams,filterParams);
        var res = all.ToMappedPagination<Lawyer, LawyerDto>(_mapper,pagedParams.PageSize);
        return Ok(new AppResponse<Pagination<LawyerDto>>(res, "success"));
    }
    [HttpGet]
    public async Task<IActionResult> GetAllUnverfiedLawyers([FromQuery] PagedParams pagedParams, [FromQuery] SortParams sortParams)
    {
        _logger.LogInformation($"Get all unverified lawyers. page no:{pagedParams.PageNumber} page size:{pagedParams.PageSize} ");
        var all = await _lawyerServices.GetAllUnverfiedLawyers(pagedParams,sortParams);
        var res = all.ToMappedPagination<Lawyer, LawyerDto>(_mapper, pagedParams.PageSize);
        return Ok(new AppResponse<Pagination<LawyerDto>>(res, "success"));

    }

}