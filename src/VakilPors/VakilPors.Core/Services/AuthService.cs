using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;
using VakilPors.Shared.Utilities;

namespace VakilPors.Core.Services;

public class AuthServices : IAuthServices
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthServices> _logger;
    private User _user;
    private readonly ISMSSender smsSender;
    private readonly IAppUnitOfWork appUnitOfWork;
    private readonly IEmailSender emailSender;
    private readonly ITelegramService _telegramService;
    public AuthServices(IMapper mapper, UserManager<User> userManager, IConfiguration configuration, ILogger<AuthServices> logger, ISMSSender smsSender, IAppUnitOfWork appUnitOfWork, IEmailSender emailSender, ITelegramService telegramService, User user = null)
    {
        this.appUnitOfWork = appUnitOfWork;
        this.emailSender = emailSender;
        this.smsSender = smsSender;
        this._mapper = mapper;
        this._userManager = userManager;
        this._configuration = configuration;
        this._logger = logger;
        this._telegramService = telegramService;
        this._user = user ?? null;
    }

    public async Task<string> CreateRefreshToken()
    {
        var newRefreshToken = Guid.NewGuid().ToString().Replace("-", "");
        _user.RefreshToken = newRefreshToken;
        _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtSettings:RefreshTokenValidityInDays"]));
        var result = await _userManager.UpdateAsync(_user);
        return newRefreshToken;
    }

    public async Task<LoginResponseDto> Login(LoginDto loginDto)
    {
        _logger.LogInformation($"Looking for user with phone number {loginDto.PhoneNumber}");
        _user = await _userManager.FindByNameAsync(loginDto.PhoneNumber);
        if (_user == null)
        {
            _logger.LogWarning($"User with phone number {loginDto.PhoneNumber} was not found");
            throw new BadArgumentException("incorrect credtials");
        }
        bool isValidUser = await _userManager.CheckPasswordAsync(_user, loginDto.Password);
        if (isValidUser == false)
        {
            _logger.LogWarning($"User with phone number {loginDto.PhoneNumber} entered wrong password");
            throw new BadArgumentException("incorrect credtials");
        }
        var token = await GenerateToken();
        _logger.LogInformation($"Token generated for user with phone number {loginDto.PhoneNumber} | Token: {token}");
        DateTime dateTime = DateTime.Now; PersianCalendar persianCalendar = new PersianCalendar();
        int year = persianCalendar.GetYear(dateTime); int month = persianCalendar.GetMonth(dateTime); int day = persianCalendar.GetDayOfMonth(dateTime); string persianDate = $"{year}/{month}/{day}";
        await _telegramService.SendToTelegram($"شما در تاریخ {persianDate} به وب سایت وکیل پرس وارد شده اید", _user.Telegram);
        await emailSender.SendEmailAsync(_user.Email, _user.Name, "ورود", $"شما در تاریخ {persianDate} به وب سایت وکیل پرس وارد شده اید");
        return new LoginResponseDto
        {
            Token = token,
            RefreshToken = await CreateRefreshToken()
        };
    }

    public async Task<IEnumerable<IdentityError>> Register(SignUpDto userDto)
    {
        _user = _mapper.Map<User>(userDto);
        _user.UserName = userDto.PhoneNumber;

        var result = await _userManager.CreateAsync(_user, userDto.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(_user, userDto.IsVakil ? RoleNames.Vakil : RoleNames.User);
            var userFromDb = await _userManager.FindByNameAsync(_user.UserName);
            if (userDto.IsVakil)
            {

                var lawyer = new Lawyer()
                {
                    UserId = userFromDb.Id,
                };
                await appUnitOfWork.LawyerRepo.AddAsync(lawyer);
                await appUnitOfWork.SaveChangesAsync();
                userFromDb.LawyerId = lawyer.Id;
                appUnitOfWork.UserRepo.Update(userFromDb);
                await appUnitOfWork.SaveChangesAsync();
            }
            var sub = new Subscribed()
            {
                UserId = userFromDb.Id,
                PremiumID = 1,
                Premium = null,
                User = null,
            };
            int id = appUnitOfWork.SubscribedRepo.AsQueryable().Max(s => s.ID) + 1;
            int maxTry = 100;
            while (maxTry-- > 0)
            {
                try
                {
                    sub.ID = id;
                    await appUnitOfWork.SubscribedRepo.AddAsync(sub);
                    await appUnitOfWork.SaveChangesAsync();
                    break;
                }
                catch (Exception)
                {
                    id = appUnitOfWork.SubscribedRepo.AsQueryable().Max(s => s.ID) + 1;
                }

            }
            await SendActivationCode(_user.PhoneNumber);
        }


        return result.Errors;
    }

    public async Task<LoginResponseDto> VerifyRefreshToken(LoginResponseDto request)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken? tokenContent;
        try
        {
            tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);
        }
        catch (System.ArgumentException)
        {
            throw new BadArgumentException("Invalid Token");
        }
        var username = tokenContent.Claims.ToList().FirstOrDefault(q => q.Type == JwtRegisteredClaimNames.Sub)?.Value;

        _user = await _userManager.FindByNameAsync(username);

        if (_user == null)
        {
            throw new NotFoundException("User not found");
        }
        _logger.LogInformation($"Refresh Token Attempt for {username} ");

        var isValidRefreshToken = _user.RefreshTokenExpiryTime >= DateTime.Now && _user.RefreshToken.Equals(request.RefreshToken);

        if (isValidRefreshToken)
        {
            var token = await GenerateToken();
            _logger.LogInformation($"token refreshed for {username}");
            return new LoginResponseDto
            {
                Token = token,
                RefreshToken = await CreateRefreshToken()
            };
        }

        _logger.LogInformation($"Invalid Token for {username} ");
        await _userManager.UpdateSecurityStampAsync(_user);
        throw new BadArgumentException("Invalid Refresh Token");
    }

    private async Task<string> GenerateToken()
    {
        var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));

        var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(_user);
        var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
        var userClaims = await _userManager.GetClaimsAsync(_user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, _user.PhoneNumber),
            new Claim(JwtRegisteredClaimNames.Name, _user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, _user.Email),
            new Claim("uid", _user.Id.ToString()),
        }
        .Union(userClaims).Union(roleClaims);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:TokenValidityInMinutes"])),
            signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public async Task CreateAndSendForgetPasswordToken(ForgetPasswordDto forgetPasswordDto)
    {
        User _user;

        if (forgetPasswordDto.useSms)
        {
            _user = await _userManager.FindByNameAsync(forgetPasswordDto.PhoneNumber);
        }
        else
        {
            _user = await _userManager.FindByEmailAsync(forgetPasswordDto.Email);
        }

        if (_user == null)
        {
            throw new NotFoundException("no user found with this phoneNumber/Email.");
        }
        //generating token 
        var code = RandomEngine.Next(100000, 1000000).ToString();
        if (forgetPasswordDto.useSms)
            _logger.LogInformation($"forget password code:{code} generated for user with phone number:{forgetPasswordDto.PhoneNumber}");
        else
            _logger.LogInformation($"forget password code:{code} generated for user with email:{forgetPasswordDto.Email}");
        _user.ForgetPasswordCode = code;
        await _userManager.UpdateAsync(_user);

        if (forgetPasswordDto.useSms)
            await smsSender.SendSmsAsync(forgetPasswordDto.PhoneNumber, $"کد بازیابی رمز عبور شما: {code} است");
        else
            await emailSender.SendEmailAsync(forgetPasswordDto.Email, _user.Name, "فراموشی رمز عبور", $"کد بازیابی رمز عبور شما: {code} است");
        await _telegramService.SendToTelegram($"کد بازیابی رمز عبور شما: {code} است", _user.Telegram);
    }

    public async Task ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        User _user;
        if (resetPasswordDto.usePhoneNumber)
        {
            _user = await _userManager.FindByNameAsync(resetPasswordDto.PhoneNumber);
        }
        else
        {
            _user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        }
        if (_user == null)
        {
            throw new NotFoundException("no user found with this phoneNumber/Email.");
        }
        if (_user.ForgetPasswordCode == null)
        {
            throw new BadArgumentException("No code has been generated for this user!");
        }
        if (resetPasswordDto.ConfirmPassword != resetPasswordDto.NewPassword)
        {
            throw new BadArgumentException("passwords don't match");
        }
        if (_user.ForgetPasswordCode != resetPasswordDto.Code)
        {
            throw new BadArgumentException("invalid code");
        }
        _user.PasswordHash = _userManager.PasswordHasher.HashPassword(_user, resetPasswordDto.NewPassword);
        _user.ForgetPasswordCode = null;
        //update user
        var result = await _userManager.UpdateAsync(_user);
        // var result = await _userManager.ResetPasswordAsync(_user,resetPasswordDto.Code,resetPasswordDto.NewPassword);
        if (!result.Succeeded)
        {
            throw new InternalServerException("password reset failed");
        }
    }

    public async Task SendActivationCode(string phoneNumber)
    {
        string code = GenerateActivationCode();
        await SetActivationCode(phoneNumber, code);
        _logger.LogInformation($"activation code:{code} generated for user with phone number:{phoneNumber}");
        await SendActivationCodeSms(phoneNumber, code);
    }

    private string GenerateActivationCode()
        => RandomEngine.Next(100000, 1000000).ToString();

    private async Task SendActivationCodeSms(string phoneNumber, string code)
    {
        var message = $"کد فعال سازی اکانت شما: {code} است";
        try
        {
            await smsSender.SendSmsAsync(phoneNumber, message);
        }
        catch (System.Exception)
        {
            throw new InternalServerException("sms sending failed");
        }
    }

    private async Task SetActivationCode(string phoneNumber, string code)
    {
        var user = await _userManager.FindByNameAsync(phoneNumber);
        if (user == null)
            throw new NotFoundException("no user found with this phone number");
        if (user.IsActive)
            throw new BadArgumentException("user is already activated!");
        user.ActivationCode = code;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new InternalServerException("set activation code failed");
    }


    public async Task ActivateAccount(ActivateAccountDto activateAccountDto)
    {
        var user = await _userManager.FindByNameAsync(activateAccountDto.PhoneNumber);
        if (user == null)
            throw new NotFoundException("no user found with this phone number");

        if (user.ActivationCode != activateAccountDto.Code)
            throw new BadArgumentException("activation code is wrong");

        user.IsActive = true;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new InternalServerException("activate account failed");
        }
    }

}
