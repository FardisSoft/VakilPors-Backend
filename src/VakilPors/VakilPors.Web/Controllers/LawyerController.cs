using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Params;
using X.PagedList;

namespace VakilPors.Web.Controllers
{
    
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
    }
}