using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Web.Controllers;
using VakilPors.Core.Contracts.Services;
using AutoMapper;
using Pagination.EntityFrameworkCore.Extensions;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.Rate;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;
using VakilPors.Core.Mapper;

namespace VakilPors.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RateController : MyControllerBase
    {
        private readonly IRateService _RateService;
        private readonly ILogger<PremiumController> _logger;
        private readonly IMapper _mapper;

        public RateController(IRateService rateService, ILogger<PremiumController> logger, IMapper mapper)
        {
            _RateService = rateService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("GetRate")]
        public async Task<IActionResult> GetRateStatus(int laywer_id)
        {
            int user_id = getUserId();
            var rate = await _RateService.GetRateAsync(user_id, laywer_id);
            if (rate == null)
            {
                return NotFound("Not found!!");
            }
            else
                return Ok(rate);
        }

        [HttpGet]
        [Route("RateAverage")]
        public async Task<double> GetAverageStatus(int lawyer_id)
        {
            double avg = await _RateService.CalculateRatingAsync(lawyer_id);
            return avg;
        }

        [HttpPost]
        [Route("AddRate")]
        public async Task<IActionResult> AddRate(RateDto rate, [FromQuery] int laywer_id)
        {
            var user_id = getUserId();
            await _RateService.AddRateAsync(rate, user_id, laywer_id);
            return Ok();
        }

        [HttpGet]
        [Route("GetRatesPaged")]
        public async Task<Pagination<RateUserDto>> GetAllRates([FromQuery] int lawyerId,
            [FromQuery] PagedParams pagedParams)
        {
            _logger.LogInformation($"user {getPhoneNumber()} get rates of lawyer {lawyerId}.");
            var rates = await _RateService.GetRatesPagedAsync(lawyerId, pagedParams);
            var rateUserDtos = rates.ToMappedPagination<Rate, RateUserDto>(_mapper, pagedParams.PageSize);
            return rateUserDtos;
        }

        [HttpPut]
        [Route("UpdateRate")]
        public async Task<IActionResult> UpdateRate(RateDto rate, [FromQuery] int laywer_id)
        {
            var user_id = getUserId();
            await _RateService.UpdateRateAsync(rate, user_id, laywer_id);
            return Ok();
        }
    }
}