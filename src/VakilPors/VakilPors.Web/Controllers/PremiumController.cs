using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
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
        [Route("GetSubscriptionStatus")]
        public async Task<ActionResult<SubscribedDto>> GetPremiumStatus()
        {
            int user_id = getUserId();
            _logger.LogInformation($"GetSubsriptionStatus for the user by id {user_id}");
            var result = await _PremiumServices.GetPremiumStatus(user_id);
            if (result == null)
                return Ok(new AppResponse<object>(result, "user is not subscribed"));
            else
                return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpPost]
        [Route("ActivateSubscription")]
        public async Task<ActionResult> ActivatePremium(string PremiumPlan)
        {
            int user_id = getUserId();
            _logger.LogInformation($"activating subscriptions for user by id{user_id} ");
            await _PremiumServices.ActivatePremium( PremiumPlan.ToLower(),user_id);
            return Ok(new AppResponse<object>(PremiumPlan, "success"));
            
        }

        [HttpPut]
        [Route("DeactiveSubscription")]
        public async Task DeactivePremium()
        {
            int user_id = getUserId();
            _logger.LogInformation($"deactivating subscriptions for user {user_id}");
            await _PremiumServices.DeactivatePremium(user_id);
        }
        //[HttpPut]
        //[Route("UpgradeSubscription")]
        //public async Task UpgradePremium(SubscribedDto subscribedDto)
        //{
        //    _logger.LogInformation($"UpgradeSubscription for user {subscribedDto.User.Id}");
        //    await _PremiumServices.UpdatePlan(subscribedDto);
        //}

    }
}
