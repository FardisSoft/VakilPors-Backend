using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Mapper;
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
        public async Task<ActionResult<IPagedList<LawyerDto>>> GetAllPaged([FromQuery] PagedParams pagedParams,[FromQuery] FilterParams filterParams){
            var all=await lawyerServices.GetLawyers(pagedParams,filterParams);
            var res=all.ToMappedPagedList<Lawyer,LawyerDto>(mapper); 
            return Ok(res);
        }
    }
}