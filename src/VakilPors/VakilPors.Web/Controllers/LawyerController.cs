using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Entities;
using X.PagedList;

namespace VakilPors.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LawyerController : MyControllerBase
    {
        private readonly ILawyerServices lawyerServices;
        private readonly IMapper mapper;

        public LawyerController(ILawyerServices lawyerServices,IMapper mapper)
        {
            this.mapper = mapper;
            this.lawyerServices = lawyerServices;
        }
        [HttpGet("GetAll")]
        public async Task<IPagedList<LawyerDto>> GetAllPaged([FromQuery] PagedParams pagedParams,[FromQuery] FilterParams filterParams){
            var all=await lawyerServices.GetLawyers(pagedParams,filterParams);
            return mapper.Map<IPagedList<LawyerDto>>(all);
        }
        [HttpGet("GetByID")]
        public async Task<LawyerDto> GetLawyer([FromQuery] int id)
        {
            var lawyer = await lawyerServices.GetLawyerByID(id);
            return mapper.Map<LawyerDto>(lawyer);
        }
        [HttpPut("EditLawyer")]
        public async Task EditLawyer([FromQuery] int id, [FromBody] LawyerDto lawyerDto)
        {
            //var lawyer = mapper.Map<Lawyer>(lawyerDto);
            await lawyerServices.EditLawyer(id, lawyerDto);
        }

    }
}