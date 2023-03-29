using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos;

namespace VakilPors.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authManager;
        private readonly ILogger<AuthController> _logger;
        ForgetPasswordDto _passwordDto;

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
            return BadRequest(ModelState);
        }

        return Ok();
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

        if (authResponse == null)
        {
            return Unauthorized();
        }

        return Ok(authResponse);

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

        if (authResponse == null)
        {
            return Unauthorized("Invalid refresh token or jwt token is expired or userid is wrong");
        }

        return Ok(authResponse);
    }
    [HttpGet]
    [Route("forgetpassword")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ForgetPassword([FromBody] ForgetPasswordDto forgetPasswordDto)
    {
         this._passwordDto = forgetPasswordDto;
         var token = await _authManager.CreateToken(forgetPasswordDto);
        /// TODO:implement  Sending SMS   
        return Ok();
    }
    [HttpPost]
    [Route("resetpassword")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        

        return Ok();
    }
    
    }
}