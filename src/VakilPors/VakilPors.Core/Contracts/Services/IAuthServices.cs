using Microsoft.AspNetCore.Identity;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services;

public interface IAuthServices : IScopedDependency
{
    
        Task<IEnumerable<IdentityError>> Register(SignUpDto userDto);
        Task<LoginResponseDto> Login(LoginDto loginDto);
        Task<string> CreateRefreshToken();
        Task<LoginResponseDto> VerifyRefreshToken(LoginResponseDto request);
        Task CreateAndSendForgetPasswordToken(ForgetPasswordDto forgetPasswordDto);
        Task ResetPassword(ResetPasswordDto resetPasswordDto);
        Task ActivateAccount(ActivateAccountDto activateAccountDto);
        Task SendActivationCode(string phoneNumber);

}

