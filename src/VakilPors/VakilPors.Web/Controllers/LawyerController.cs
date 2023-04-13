using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using X.PagedList;

namespace VakilPors.Web.Controllers
{
    
    public class LawyerController : MyControllerBase
    {
        private readonly ILawyerServices _lawyerServices;
        private readonly IMapper _mapper;
        private readonly ILogger<LawyerController> _logger;

        public LawyerController(ILawyerServices lawyerServices,IMapper mapper, ILogger<LawyerController> logger)
        {
            this._mapper = mapper;
            _logger = logger;
            this._lawyerServices = lawyerServices;
        }
        [HttpGet("GetAll")]
        public async Task<IPagedList<LawyerDto>> GetAllPaged([FromQuery] PagedParams pagedParams,[FromQuery] FilterParams filterParams){
            var all=await _lawyerServices.GetLawyers(pagedParams,filterParams);
            return _mapper.Map<IPagedList<LawyerDto>>(all);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateLawyer(LawyerDto lawyerDto)
        {
            _logger.LogInformation($"update lawyer attempt for {lawyerDto.User.UserName ?? lawyerDto.Id.ToString()}");
            var result = await _lawyerServices.UpdateLawyer(lawyerDto);
            return Ok(new AppResponse<object>(result, "Profile Updated"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLawyers()
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
        [Authorize]
        public async Task<IActionResult> GetCurrentLawyer()
        {
            _logger.LogInformation($"get current lawyer data");
            var result = await _lawyerServices.GetLawyerByUserId(getUserId());
            return Ok(new AppResponse<object>(result, "success"));
        }
}
}