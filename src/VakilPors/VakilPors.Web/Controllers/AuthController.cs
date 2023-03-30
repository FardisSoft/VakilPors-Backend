using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Shared.Response;

namespace VakilPors.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthServices authManager, ILogger<AuthController> logger)
        {
            this._authManager = authManager;
            this._logger = logger;
        }

        // POST: api/Account/register
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Register([FromBody] SignUpDto apiUserDto)
        {
            _logger.LogInformation($"Registration Attempt for {apiUserDto.PhoneNumber}");
            var errors = await _authManager.Register(apiUserDto);

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(new AppResponse<ModelStateDictionary>(ModelState, $"Fields validations resulted in errors!", HttpStatusCode.BadRequest));
            }

            return Ok(new AppResponse(HttpStatusCode.OK, $"User with phone number {apiUserDto.PhoneNumber} has been Successfully Registered!"));
        }

        // POST: api/Account/login
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation($"Login Attempt for {loginDto.PhoneNumber} ");
            var authResponse = await _authManager.Login(loginDto);

            return Ok(new AppResponse<LoginResponseDto>(authResponse, $"User with phone number {loginDto.PhoneNumber} tokens have been generated!"));

        }

        // POST: api/Account/refreshtoken
        [HttpPost]
        [Route("refreshtoken")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RefreshToken([FromBody] LoginResponseDto request)
        {
            var authResponse = await _authManager.VerifyRefreshToken(request);

            return Ok(new AppResponse<LoginResponseDto>(authResponse, $"tokens have been generated!"));
        }
        [HttpGet]
        [Route("forgetpassword")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> ForgetPassword([FromQuery] ForgetPasswordDto forgetPasswordDto)
        {
            await _authManager.CreateForgetPasswordToken(forgetPasswordDto);
            // TODO:implement Sending SMS   


            return Ok(new AppResponse(HttpStatusCode.OK,"Forget Password Code sent to your phone number!")); //200
        }

        [HttpPost]
        [Route("resetpassword")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AppResponse<ModelStateDictionary>(ModelState, $"Fields validations resulted in errors!", HttpStatusCode.BadRequest));
            }
            await _authManager.ResetPassword(resetPasswordDto);
            return Ok(new AppResponse(HttpStatusCode.OK, $"Password has been reset!"));    
        }


    }
}