using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Domain.Entities;

namespace VakilPors.Core.Services;

public class AuthServices : IAuthServices
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthServices> _logger;
    private User _user;

    public AuthServices(IMapper mapper, UserManager<User> userManager, IConfiguration configuration, ILogger<AuthServices> logger)
    {
        this._mapper = mapper;
        this._userManager = userManager;
        this._configuration = configuration;
        this._logger = logger;

    }

    public async Task<string> CreateRefreshToken()
    {
        var newRefreshToken = Guid.NewGuid().ToString().Replace("-","");
        _user.RefreshToken=newRefreshToken;
        _user.RefreshTokenExpiryTime=DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtSettings:RefreshTokenValidityInDays"]));
        var result = await _userManager.UpdateAsync(_user);
        return newRefreshToken;
    }

    public async Task<LoginResponseDto> Login(LoginDto loginDto)
    {
        _logger.LogInformation($"Looking for user with phone number {loginDto.PhoneNumber}");
        _user = await _userManager.FindByNameAsync(loginDto.PhoneNumber);
        bool isValidUser = await _userManager.CheckPasswordAsync(_user, loginDto.Password);

        if (_user == null || isValidUser == false)
        {
            _logger.LogWarning($"User with phone number {loginDto.PhoneNumber} was not found or password was wrong");
            return null;
        }

        var token = await GenerateToken();
        _logger.LogInformation($"Token generated for user with phone number {loginDto.PhoneNumber} | Token: {token}");

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
        _user.Email="null@null.com";

        var result = await _userManager.CreateAsync(_user, userDto.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(_user, RoleNames.User);
        }

        return result.Errors;
    }

    public async Task<LoginResponseDto> VerifyRefreshToken(LoginResponseDto request)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken? tokenContent;
        try
        {
            tokenContent= jwtSecurityTokenHandler.ReadJwtToken(request.Token);
        }
        catch (System.ArgumentException)
        {
            return null;
        } 
        var username = tokenContent.Claims.ToList().FirstOrDefault(q => q.Type == JwtRegisteredClaimNames.Email)?.Value;
        _user = await _userManager.FindByNameAsync(username);

        if (_user == null)
        {
            return null;
        }

        var isValidRefreshToken = _user.RefreshTokenExpiryTime >= DateTime.Now &&
        _user.RefreshToken.Equals(request.RefreshToken);

        if (isValidRefreshToken)
        {
            var token = await GenerateToken();
            return new LoginResponseDto
            {
                Token = token,
                RefreshToken = await CreateRefreshToken()
            };
        }

        await _userManager.UpdateSecurityStampAsync(_user);
        return null;
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
            new Claim(JwtRegisteredClaimNames.Email, _user.PhoneNumber),
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
    public async Task<string> CreateToken(ForgetPasswordDto forgetPasswordDto)
    {
        string phone = forgetPasswordDto.PhoneNumber;
        _user = _mapper.Map<User>(forgetPasswordDto);
        var token = this._userManager.CreateSecurityTokenAsync(_user).ToString();
        return token;
    }

    public Task ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        string phone = resetPasswordDto.PhoneNumber;
        // getting the user by phone number
        _user = _userManager.FindByNameAsync(phone).Result;
        // reseting the password
        var result = _userManager.ResetPasswordAsync(_user, resetPasswordDto.Code, resetPasswordDto.New_Password).Result;
        throw new NotImplementedException();
    }

}
