﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Domain.Dtos.Premium;
using VakilPors.Shared.Response;



namespace VakilPors.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PremiumController :MyControllerBase
    {
        private readonly IPremiumService _PremiumServices;
        private readonly ILogger<PremiumController> _logger;
        private readonly IMapper _mapper;

        public PremiumController(IPremiumService premiumServices, ILogger<PremiumController> logger, IMapper mapper)
        {
            _PremiumServices = premiumServices;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("GetPremiumStatus")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PremiumDto>> GetPremiumStatus()
        {
            int user_id = getUserId();
            _logger.LogInformation($"GetPremiumStatus for the user by id {user_id}");
            var result = await _PremiumServices.GetPremiumStatus(user_id);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpPost]
        [Route("ActivatePremium")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task ActivatePremium(PremiumDto premium)
        {
            int user_id = getUserId();
            _logger.LogInformation($"activating premium for user by id{user_id} ");
            await _PremiumServices.ActivatePremium(premium, user_id);
        }

        [HttpPut]
        [Route("DeactivePremium")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task DeactivePremium()
        {
            int user_id = getUserId();
            _logger.LogInformation($"deactivating premium for user {user_id}");
            await _PremiumServices.DeactivatePremium(user_id);

        }

    }
}
