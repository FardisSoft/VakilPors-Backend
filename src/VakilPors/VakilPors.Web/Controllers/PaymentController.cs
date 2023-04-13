using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Payment;

namespace VakilPors.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : MyControllerBase
    {
        private readonly IPaymentServices paymentServices;
        public PaymentController(IPaymentServices paymentServices)
        {
            this.paymentServices = paymentServices;

        }
        [HttpPost("request")]
        public async Task<IActionResult> RequestPayment([FromBody]RequestPaymentDto requestPaymentDto)
        {
            var baseRoute = getBaseRoute();
            var Route=Url.Action(nameof(VerifyPayment)) ?? "/payment/verify";
            var callbackUrl = baseRoute + Route;
            var userId = getUserId();
            return Ok(await paymentServices.RequestPayment(userId, requestPaymentDto.Amount, requestPaymentDto.Description, callbackUrl));
        }


        [AllowAnonymous]
        [HttpGet("verify")]
        public async Task<IActionResult> VerifyPayment(string authority, string status)
        {
            var res = await paymentServices.VerifyPayment(authority, status);
            var baseRoute = getBaseRoute().Replace("api.", "");
            //Todo: change this to react route in production
            var Route="/payment/verify";
            var url = baseRoute + Route;
            return Redirect($"{url}?{getQueryString(res)}");
        }
        
    }
}