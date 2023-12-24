using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Payment;

namespace VakilPors.Api.Controllers
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
        public async Task<IActionResult> RequestPayment([FromBody] RequestPaymentDto requestPaymentDto)
        {
            var baseRoute = GetBaseRoute();
            var Route = Url.Action(nameof(VerifyPayment)) ?? "/payment/verify";
            var callbackUrl = baseRoute + Route;
            var userId = GetUserId();
            return Ok(await paymentServices.RequestPayment(userId, requestPaymentDto.Amount, requestPaymentDto.Description, callbackUrl));
        }


        [AllowAnonymous]
        [HttpGet("verify")]
        public async Task<IActionResult> VerifyPayment(string authority, string status)
        {
            var res = await paymentServices.VerifyPayment(authority, status);
            var baseRoute = GetBaseRoute().Replace("api.", "");
            var Route = "/payment/verify";
            var url = baseRoute + Route;
            return Redirect($"{url}?{GetQueryString(res)}");
        }

    }
}