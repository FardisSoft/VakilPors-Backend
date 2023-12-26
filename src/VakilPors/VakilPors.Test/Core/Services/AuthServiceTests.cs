using Moq;
using MockQueryable.Moq;
using Microsoft.AspNetCore.Identity;
using VakilPors.Core.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using VakilPors.Core.Services;
using VakilPors.Core.Contracts.Services;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Exceptions;
using System.Reflection;
using System.Security.Claims;
using VakilPors.Contracts.Repositories;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace VakilPors.Test.Core.Services;
public class AuthServicesTests{
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<UserManager<User>> _userManager;
    private readonly Mock<IConfiguration> _configuration;
    private readonly Mock<ILogger<AuthServices>> _logger;
    private readonly Mock<ISMSSender> _smsSender;
    private readonly Mock<IAppUnitOfWork> _appUnitOfWork;
    private readonly Mock<IEmailSender> _emailSender;
    private readonly Mock<ITelegramService> _telegramService;
    private readonly AuthServices _authServices;
    private User _user;
    private Mock<IGenericRepo<Lawyer>> _lawyerRepMock;
    private Mock<IGenericRepo<User>> _userRepMock;
    private Mock<IGenericRepo<Subscribed>> _subscribedRepMock;
    Mock<JwtSecurityTokenHandler> _jwtSecurityTokenHandlerMock ;


    // public Fixture fixture = new Fixture();


    public AuthServicesTests(){
        _lawyerRepMock= new Mock<IGenericRepo<Lawyer>>();

         _userRepMock= new Mock<IGenericRepo<User>>();
         _subscribedRepMock= new Mock<IGenericRepo<Subscribed>>();
        // _user= fixture.Create<User>();
        _user = new User
        {
            UserName="username",
            PhoneNumber = "testPhoneNumber",
            Name = "Test User",
            Id =1,// int.Parse(Guid.NewGuid().ToString()),
            Email = "test@example.com",
            Telegram = "telegram"
            // Set other properties as needed for your test case
        };
        _mapper = new Mock<IMapper>();
        // _userManager = new Mock<UserManager<User>>();
        _userManager = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(),
            null,null,null,null,null,null,null,null
        );
        _configuration = new Mock<IConfiguration>();
        _logger = new Mock<ILogger<AuthServices>>();
        _smsSender = new Mock<ISMSSender>();
        _appUnitOfWork = new Mock<IAppUnitOfWork>();
        _emailSender = new Mock<IEmailSender>();
        _telegramService = new Mock<ITelegramService>();
        _authServices = new AuthServices(_mapper.Object, _userManager.Object, _configuration.Object, _logger.Object, _smsSender.Object, _appUnitOfWork.Object, _emailSender.Object, _telegramService.Object);
        _appUnitOfWork.Setup(uow => uow.LawyerRepo).Returns(_lawyerRepMock.Object);
        _appUnitOfWork.Setup(uow => uow.UserRepo).Returns(_userRepMock.Object);
        _appUnitOfWork.Setup(uow => uow.SubscribedRepo).Returns(_subscribedRepMock.Object);
        _jwtSecurityTokenHandlerMock= new Mock<JwtSecurityTokenHandler>();

    }
    [Fact]
    public async void CreateRefreshToken_ReallyToken_ReturnNotNullToken(){
        _userManager.Setup(x => x.UpdateAsync(It.IsAny<User>()))
        .ReturnsAsync(IdentityResult.Success);

        
        // Act
        var newToken = await _authServices.CreateRefreshToken(_user);

        // Assert 
        Assert.NotNull(newToken);
        _userManager.Verify(x=>x.UpdateAsync(It.IsAny<User>()), Times.Once);

    }

    // public async void Login_CantFindUserByName_ThrowBadArgumentException(){

    // }
    [Fact]
    public async Task Login_CantFindUserByName_ThrowBadArgumentException()
    {
        // var capturedLogs = new List<string>();
        // _logger.Setup(x => x.Log(
        //     It.IsAny<LogLevel>(),
        //     It.IsAny<EventId>(),
        //     It.IsAny<It.IsAnyType>(),
        //     It.IsAny<Exception>(),
        //     (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
        //     ))
        //     .Callback<LogLevel, EventId, It.IsAnyType, Exception, Func<It.IsAnyType, Exception, string>>((logLevel, eventId, state, exception, formatter) =>
        //     {
        //         capturedLogs.Add(formatter(state, exception));
        //     });
        // Arrange
        var Phone = "1234567890";
        
        _userManager.Setup(x => x.FindByNameAsync(Phone))
                   .ReturnsAsync((User)null);
                   
        // Act
        // Func<Task> act = async () => 
        //     await _authServices.Login(new LoginDto { PhoneNumber = Phone });        
        // // Assert 
        // await Assert.ThrowsAsync<BadArgumentException>(act);
        // OR
        await Assert.ThrowsAsync<BadArgumentException>(() => _authServices.Login(new LoginDto { PhoneNumber = Phone }));

        
        // _logger.VerifyLogging(
        //     LogLevel.Warning,  
        //     $"User with phone {Phone} not found"
        // );

        // _logger.Verify(x => x.LogInformation($"Looking for user with phone number {Phone}"), Times.Once);
        // _logger.Verify(x => x.LogWarning($"User with phone number {Phone} was not found"), Times.Once);
            
        _userManager.Verify(x=> x.FindByNameAsync(Phone), Times.Once);
    }
    [Fact]
    public async Task Login_NotValidUser_ThrowBadArgumentException(){
        User user = new User();
        _userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                   .ReturnsAsync(user);
        var Phone = "1234567890";
        string password ="password";
        _userManager.Setup(x=>x.CheckPasswordAsync(user , password)).ReturnsAsync(false);

        await Assert.ThrowsAsync<BadArgumentException>(() => _authServices.Login(new LoginDto { PhoneNumber = Phone,Password=password }));
    }    
    [Fact]
    public async Task Login_Successfully_DoAllTask()
    {
        // Arrange
        var user = new User
        {
            PhoneNumber = "testPhoneNumber",
            Name = "Test User",
            Id =1,// int.Parse(Guid.NewGuid().ToString()),
            Email = "test@example.com",
            Telegram = "telegram"
            // Set other properties as needed for your test case
        };
        string password ="password";
        _userManager.Setup(x=>x.CheckPasswordAsync(user , password)).ReturnsAsync(true);
        _userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                       .ReturnsAsync(user);

        _userManager.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                       .ReturnsAsync(new List<string> { "UserRole" });

        _userManager.Setup(x => x.GetClaimsAsync(It.IsAny<User>()))
                       .ReturnsAsync(new List<Claim> { new Claim("customClaim", "customValue") });

        _configuration.Setup(x => x["JwtSettings:Key"]).Returns("your_secret_key_here");
        _configuration.Setup(x => x["JwtSettings:Issuer"]).Returns("your_issuer");
        _configuration.Setup(x => x["JwtSettings:Audience"]).Returns("your_audience");
        _configuration.Setup(x => x["JwtSettings:TokenValidityInMinutes"]).Returns("60");

        // Act
        var result = await _authServices.Login(new LoginDto { PhoneNumber = "testPhoneNumber" ,Password=password });

        // Assert
        Assert.NotNull(result);
        _telegramService.Verify(x => x.SendToTelegram(It.IsAny<string>(), user.Telegram), Times.Once);
        _emailSender.Verify(x => x.SendEmailAsync(user.Email, user.Name, It.IsAny<string>(), It.IsAny<string>() , true), Times.Once);


        // Decode and validate the generated token
        // var handler = new JwtSecurityTokenHandler();
        // var jsonToken = handler.ReadToken(result) as JwtSecurityToken;

        // Assert.NotNull(jsonToken);
        // Assert.Equal("testPhoneNumber", jsonToken.Subject);
        // Assert.Equal("Test User", jsonToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value);
        // Assert.Equal(user.Email, jsonToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
        // Assert.Contains(jsonToken.Claims, c => c.Type == "uid" && c.Value == user.Id);
        // Assert.Contains(jsonToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == "UserRole");
        // Assert.Contains(jsonToken.Claims, c => c.Type == "customClaim" && c.Value == "customValue");
    }
    [Fact]
    public async Task Register_CantCreateAsync_ReturnError(){
        SignUpDto sud =new SignUpDto{PhoneNumber = _user.PhoneNumber , Email=_user.Email , IsVakil = true,Name=_user.Name};// fixture.Create<SignUpDto>();
        var errorDescription = "Error creating user";
        var identityError = new IdentityError { Code = "ErrorCode", Description = errorDescription };
        _userManager.Setup(x => x.CreateAsync(_user,sud.Password))
                       .ReturnsAsync(IdentityResult.Failed(identityError));
        _mapper.Setup(x => x.Map<User>(It.IsAny<SignUpDto>()))
                       .Returns(_user);


        var result = await _authServices.Register(sud);

        Assert.Equal(errorDescription, result.First().Description);

    }
    [Fact]
     public async Task Register_CantCreateAsync_ReturnErdror(){
        var phoneNumber = "1234567890"; // Replace with a valid phone number
        var activationCode = "123456";   // Replace with a generated activation code

        _userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new User { IsActive = false , Id = 2}); // Assuming user is not yet activated

        _userManager.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success);

        _smsSender.Setup(x => x.SendSmsAsync(phoneNumber, It.IsAny<string>()));
        
        SignUpDto sud =new SignUpDto{PhoneNumber = _user.PhoneNumber , Email=_user.Email , IsVakil = true,Name=_user.Name};// fixture.Create<SignUpDto>();
        sud.IsVakil = false;
        // var errorDescription = "Error creating user";
        // var identityError = new IdentityError { Code = "ErrorCode", Description = errorDescription };
        _userManager.Setup(x => x.CreateAsync(_user,sud.Password))
                       .ReturnsAsync(IdentityResult.Success);// Failed(identityError));
        _mapper.Setup(x => x.Map<User>(It.IsAny<SignUpDto>()))
                       .Returns(_user);

        _appUnitOfWork.Setup(um => um.SaveChangesAsync()).ThrowsAsync(new Exception());
        _lawyerRepMock.Setup(lrm =>lrm.AddAsync(It.IsAny<Lawyer>()));
        _userRepMock.Setup(urm => urm.Update(It.IsAny<User>()));

        _subscribedRepMock.Setup(srm => srm.AddAsync(It.IsAny<Subscribed>()));
        var s = new List<Subscribed>{ new Subscribed{ID = 1} };
        var submock =s.BuildMock();
        _subscribedRepMock.Setup(srm => srm.AsQueryable()).Returns(submock);


        var result = await _authServices.Register(sud);

        // Assert.Equal(errorDescription, result.First().Description);

    }


    [Fact]
    public async Task SendActivationCode_Successful()
    {
        // Arrange
        var phoneNumber = "1234567890"; // Replace with a valid phone number
        var activationCode = "123456";   // Replace with a generated activation code

        _userManager.Setup(x => x.FindByNameAsync(phoneNumber))
            .ReturnsAsync(new User { IsActive = false }); // Assuming user is not yet activated

        _userManager.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success);

        _smsSender.Setup(x => x.SendSmsAsync(phoneNumber, It.IsAny<string>()));
            // .ReturnsAsync(Task.CompletedTask); // Assuming successful SMS sending

        // Act
        await _authServices.SendActivationCode(phoneNumber);

        // Assert
        // Add assertions based on your specific requirements
        _userManager.Verify(x => x.FindByNameAsync(phoneNumber), Times.Once);
        _userManager.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
        _smsSender.Verify(x => x.SendSmsAsync(phoneNumber, It.IsAny<string>()), Times.Once);
        // loggerMock.Verify(x => x.LogInformation($"activation code:{activationCode} generated for user with phone number:{phoneNumber}"), Times.Once);
    }
    [Fact]
    public async Task SendActivationCode_SendSmsAsyncFailed_ThrowInternalServerException()
    {
        // Arrange
        var phoneNumber = "1234567890"; // Replace with a valid phone number
        var activationCode = "123456";   // Replace with a generated activation code

        _userManager.Setup(x => x.FindByNameAsync(phoneNumber))
            .ReturnsAsync(new User { IsActive = false }); // Assuming user is not yet activated

        _userManager.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success);

        _smsSender.Setup(x => x.SendSmsAsync(phoneNumber, It.IsAny<string>())).ThrowsAsync(new Exception());
            // .ReturnsAsync(Task.CompletedTask); // Assuming successful SMS sending

        // Act
        // await _authServices.SendActivationCode(phoneNumber);
        await Assert.ThrowsAsync<InternalServerException>(() => _authServices.SendActivationCode(phoneNumber));


        // Assert
        _userManager.Verify(x => x.FindByNameAsync(phoneNumber), Times.Once);
        _userManager.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
        _smsSender.Verify(x => x.SendSmsAsync(phoneNumber, It.IsAny<string>()), Times.Once);
        // loggerMock.Verify(x => x.LogInformation($"activation code:{activationCode} generated for user with phone number:{phoneNumber}"), Times.Once);
    }

    [Fact]
    public async Task ActivateAccount_FindNullUser_ThrowNotFoundException()
    {
        User user=null;// =new User{};
        ActivateAccountDto aad = new ActivateAccountDto{PhoneNumber = "phone" , Code = "code"};
        _userManager.Setup(um => um.FindByNameAsync(aad.PhoneNumber)).ReturnsAsync(user);
        await Assert.ThrowsAsync<NotFoundException>(() => _authServices.ActivateAccount(aad));
    }
    [Fact]
    public async Task ActivateAccount_NotEqualityInCode_ThrowBadArgumentException()
    {
        User user=new User{ActivationCode = "invalid_code"};
        ActivateAccountDto aad = new ActivateAccountDto{PhoneNumber = "phone" , Code = "code"};
        _userManager.Setup(um => um.FindByNameAsync(aad.PhoneNumber)).ReturnsAsync(user);
        await Assert.ThrowsAsync<BadArgumentException>(() => _authServices.ActivateAccount(aad));
    }
    [Fact]
    public async Task ActivateAccount_CantUpdate_ThrowInternalServerException()
    {
        User user=new User{ActivationCode = "code"};
        ActivateAccountDto aad = new ActivateAccountDto{PhoneNumber = "phone" , Code = "code"};
        _userManager.Setup(um => um.FindByNameAsync(aad.PhoneNumber)).ReturnsAsync(user);

        var identityError = new IdentityError { Code = "ErrorCode", Description = "Error description" };
        _userManager.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Failed(identityError));

        await Assert.ThrowsAsync<InternalServerException>(() => _authServices.ActivateAccount(aad));
    }
    [Fact]
    public async Task ActivateAccount_SuccessWay_Ok()
    {
        User user=new User{ActivationCode = "code"};
        ActivateAccountDto aad = new ActivateAccountDto{PhoneNumber = "phone" , Code = "code"};
        _userManager.Setup(um => um.FindByNameAsync(aad.PhoneNumber)).ReturnsAsync(user);
        _userManager.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        await _authServices.ActivateAccount(aad);
    }

    [Fact]
    public async Task VerifyRefreshToken_CantReadJwtToken_ThrowBadArgumentException()
    {
        LoginResponseDto lrd = new LoginResponseDto{Token="fjdkslaj;"};
        await Assert.ThrowsAsync<BadArgumentException>(() => _authServices.VerifyRefreshToken(lrd));
    }
    [Fact]
    public async Task VerifyRefreshToken_CantFindUserByName_ThrowNotFoundException()
    {
        
        var token = GenerateTestToken("test_username");
        LoginResponseDto lrd = new LoginResponseDto{Token=token};
        User user =null;// new User{};
        _userManager.Setup(um => um.FindByNameAsync("test_username")).ReturnsAsync(user);
        
        await Assert.ThrowsAsync<NotFoundException>(() => _authServices.VerifyRefreshToken(lrd));

    }
    [Fact]
    public async Task VerifyRefreshToken_ExpireRefreshToken_ThrowBadArgumentException()
    { 
        
        var token = GenerateTestToken("test_username");
        LoginResponseDto lrd = new LoginResponseDto{Token=token};
        User user =new User{RefreshTokenExpiryTime = DateTime.Now.AddDays(-1)};
        _userManager.Setup(um => um.FindByNameAsync("test_username")).ReturnsAsync(user);
        
        await Assert.ThrowsAsync<BadArgumentException>(() => _authServices.VerifyRefreshToken(lrd));

    }
    [Fact]
    public async Task VerifyRefreshToken_Success_ReturnLoginResponseDto()
    { 
        _userManager.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<string> { "UserRole" });

        _userManager.Setup(x => x.GetClaimsAsync(It.IsAny<User>()))
                       .ReturnsAsync(new List<Claim> { new Claim("customClaim", "customValue") });

        _configuration.Setup(x => x["JwtSettings:Key"]).Returns("your_secret_key_here");
        _configuration.Setup(x => x["JwtSettings:Issuer"]).Returns("your_issuer");
        _configuration.Setup(x => x["JwtSettings:Audience"]).Returns("your_audience");
        _configuration.Setup(x => x["JwtSettings:TokenValidityInMinutes"]).Returns("60");

        var token = GenerateTestToken("test_username");
        LoginResponseDto lrd = new LoginResponseDto{Token=token , RefreshToken = "refresh_token"};
        User user =new User{RefreshTokenExpiryTime = DateTime.Now.AddDays(1),RefreshToken = "refresh_token" , Id = 1 , PhoneNumber="phone" , Name="name" , Email="email"};
        _userManager.Setup(um => um.FindByNameAsync("test_username")).ReturnsAsync(user);
        
        var result = await _authServices.VerifyRefreshToken(lrd);

        Assert.NotNull(result);

        Type resultType = result.GetType();

        Assert.Equal(typeof(LoginResponseDto), result.GetType());
        Assert.NotNull(result.Token);
        Assert.NotNull(result.RefreshToken);

        }
    [Fact]
    public async Task CreateAndSendForgetPasswordToken_SuccessWayUseSms_CallCorrectFunction()
    {
        // Arrange
        var forgetPasswordDto = new ForgetPasswordDto
        {
            useSms = true
        };
        _userManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                       .ReturnsAsync(new User()); // Assuming you have a User class

        // _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
        //                .ReturnsAsync(new User()); // Assuming you have a User class

        _smsSender.Setup(m => m.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()))
                     .Returns(Task.FromResult(1));

        // _emailSender.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>() , true))
        //                .Returns(Task.CompletedTask);

        _telegramService.Setup(m => m.SendToTelegram(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(Task.CompletedTask);

        // Act
        await _authServices.CreateAndSendForgetPasswordToken(forgetPasswordDto);

        // Assert
        // Add assertions as needed to verify the expected behavior
        _userManager.Verify(m => m.UpdateAsync(It.IsAny<User>()), Times.Once);
        _smsSender.Verify(m => m.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _emailSender.Verify(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>() , true), Times.Never);
        _telegramService.Verify(m => m.SendToTelegram(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
    [Fact]
    public async Task CreateAndSendForgetPasswordToken_CantFindUserByName_ThrowNotFoundException()
    {
        // Arrange
        var forgetPasswordDto = new ForgetPasswordDto
        {
            useSms = true
        };

        _userManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                       .ReturnsAsync((User)null); // Assuming you have a User class

        // Act
        await Assert.ThrowsAsync<NotFoundException>(() => _authServices.CreateAndSendForgetPasswordToken(forgetPasswordDto));
        // await _authServices.CreateAndSendForgetPasswordToken(forgetPasswordDto);
    }
    [Fact]
    public async Task CreateAndSendForgetPasswordToken_CantFindUserByEmail_ThrowNotFoundException()
    {
        // Arrange
        var forgetPasswordDto = new ForgetPasswordDto
        {
            useSms = false
        };

        _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                       .ReturnsAsync((User)null); // Assuming you have a User class

        // Act
        await Assert.ThrowsAsync<NotFoundException>(() => _authServices.CreateAndSendForgetPasswordToken(forgetPasswordDto));
        // await _authServices.CreateAndSendForgetPasswordToken(forgetPasswordDto);
    }
    [Fact]
    public async Task CreateAndSendForgetPasswordToken_SuccessWayUsemail_CallCorrectFunction()
    {
        // Arrange
        var forgetPasswordDto = new ForgetPasswordDto
        {
            useSms = false
        };

        _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                       .ReturnsAsync(new User()); // Assuming you have a User class

        _emailSender.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>() , true))
                       .Returns(Task.CompletedTask);

        _telegramService.Setup(m => m.SendToTelegram(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(Task.CompletedTask);

        // Act
        await _authServices.CreateAndSendForgetPasswordToken(forgetPasswordDto);

        // Assert
        // Add assertions as needed to verify the expected behavior
        _userManager.Verify(m => m.UpdateAsync(It.IsAny<User>()), Times.Once);
        _smsSender.Verify(m => m.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _emailSender.Verify(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>() , true), Times.Once);
        _telegramService.Verify(m => m.SendToTelegram(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_WithValidData_ShouldSucceed()
    {
        // Arrange
        Mock<IPasswordHasher<User>> _mockPasswordHasher = new Mock<IPasswordHasher<User>>();
        _userManager.Object.PasswordHasher = _mockPasswordHasher.Object;
        _mockPasswordHasher.Setup(ph => ph.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
                        .Returns("newlyHashedPassword");
                        
        var resetPasswordDto = new ResetPasswordDto
        {
            NewPassword = "ValidCode",
            Code  = "forget",
            ConfirmPassword = "ValidCode"
        };
        

        _userManager.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                       .ReturnsAsync(new User { ForgetPasswordCode = "forget",PasswordHash=""}); // Assuming you have a User class

        _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                       .ReturnsAsync(new User { ForgetPasswordCode = "forget",PasswordHash="" }); // Assuming you have a User class

        _userManager.Setup(m => m.UpdateAsync(It.IsAny<User>()))
                       .ReturnsAsync(IdentityResult.Success); // Assuming successful update

        // Act
        await _authServices.ResetPassword(resetPasswordDto);

        // Assert
        _userManager.Verify(m => m.FindByNameAsync(resetPasswordDto.PhoneNumber), Times.Once);
        _userManager.Verify(m => m.UpdateAsync(It.Is<User>(u => u.PasswordHash == "newlyHashedPassword")), Times.Once);
    
    }
    [Fact]
    public async Task ResetPassword_ValidDataWithoutPhone_ShouldSucceed()
    {
        // Arrange
        Mock<IPasswordHasher<User>> _mockPasswordHasher = new Mock<IPasswordHasher<User>>();
        _userManager.Object.PasswordHasher = _mockPasswordHasher.Object;
        _mockPasswordHasher.Setup(ph => ph.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
                        .Returns("newlyHashedPassword");
                        
        var resetPasswordDto = new ResetPasswordDto
        {
            usePhoneNumber = false,
            NewPassword = "ValidCode",
            Code  = "forget",
            ConfirmPassword = "ValidCode"
        };
        
        _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                       .ReturnsAsync(new User { ForgetPasswordCode = "forget",PasswordHash="" }); // Assuming you have a User class

        _userManager.Setup(m => m.UpdateAsync(It.IsAny<User>()))
                       .ReturnsAsync(IdentityResult.Success); // Assuming successful update

        // var yourClass = new YourClass(userManagerMock.Object);

        // Act
        await _authServices.ResetPassword(resetPasswordDto);

        // Assert
        _userManager.Verify(m => m.FindByNameAsync(resetPasswordDto.PhoneNumber), Times.Never);
        _userManager.Verify(m => m.FindByEmailAsync(resetPasswordDto.PhoneNumber), Times.Once);
        _userManager.Verify(m => m.UpdateAsync(It.Is<User>(u => u.PasswordHash == "newlyHashedPassword")), Times.Once);
    
    }
    [Fact]
    public async Task ResetPassword_FailedForUserManagerUpdate_ThrowInternalServerException()
    {
        // Arrange
        Mock<IPasswordHasher<User>> _mockPasswordHasher = new Mock<IPasswordHasher<User>>();
        _userManager.Object.PasswordHasher = _mockPasswordHasher.Object;
        _mockPasswordHasher.Setup(ph => ph.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
                        .Returns("newlyHashedPassword");
                        
        var resetPasswordDto = new ResetPasswordDto
        {
            usePhoneNumber = false,
            NewPassword = "ValidCode",
            Code  = "forget",
            ConfirmPassword = "ValidCode"
        };
        
        _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                       .ReturnsAsync(new User { ForgetPasswordCode = "forget",PasswordHash="" }); // Assuming you have a User class

        IdentityError ie = new IdentityError();
        _userManager.Setup(m => m.UpdateAsync(It.IsAny<User>()))
                       .ReturnsAsync(IdentityResult.Failed(ie)); // Assuming successful update

        // var yourClass = new YourClass(userManagerMock.Object);

        // Act
        await Assert.ThrowsAsync<InternalServerException>(() => _authServices.ResetPassword(resetPasswordDto));
    }
    [Fact]
    public async Task ResetPassword_FindByNameReturnNullUser_ThrowNotFoundException()
    {
        // Arrange
        var resetPasswordDto = new ResetPasswordDto
        {
            usePhoneNumber = false,
            NewPassword = "ValidCode",
            Code  = "forget",
            ConfirmPassword = "ValidCode"
        };
        
        _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                       .ReturnsAsync((User)null);
        

        

        // var yourClass = new YourClass(userManagerMock.Object);

        // Act
        await Assert.ThrowsAsync<NotFoundException>(() => _authServices.ResetPassword(resetPasswordDto));
    }
    [Fact]
    public async Task ResetPassword_ForgetPasswordCodeIsNull_ThrowBadArgumentException()
    {
        var resetPasswordDto = new ResetPasswordDto
        {
            usePhoneNumber = false,
            NewPassword = "ValidCode",
            Code  = "forget",
            ConfirmPassword = "ValidCode"
        };
        
        _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                       .ReturnsAsync(new User{ForgetPasswordCode = null});

        // Act
        var exception = await Assert.ThrowsAsync<BadArgumentException>(() => 
            _authServices.ResetPassword(resetPasswordDto)
        );
        
        // Assert
        Assert.Equal("No code has been generated for this user!", exception.Message);
        }
    [Fact]
    public async Task ResetPassword_ConfirmPasswordNotEquallToNewPassword_ThrowBadArgumentException()
    {
        var resetPasswordDto = new ResetPasswordDto
        {
            usePhoneNumber = false,
            NewPassword = "InValidCode",
            Code  = "forget",
            ConfirmPassword = "ValidCode"
        };
        
        _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                       .ReturnsAsync(new User{ForgetPasswordCode = "forget"});

        // Act
        var exception = await Assert.ThrowsAsync<BadArgumentException>(() => 
            _authServices.ResetPassword(resetPasswordDto)
        );
        
        // Assert
        Assert.Equal("passwords don't match", exception.Message);
        }
    [Fact]
    public async Task ResetPassword_ForgetPasswordCodeNotEquallDtoCode_ThrowBadArgumentException()
    {
        var resetPasswordDto = new ResetPasswordDto
        {
            usePhoneNumber = false,
            NewPassword = "ValidCode",
            Code  = "invalidforget",
            ConfirmPassword = "ValidCode"
        };
        
        _userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                       .ReturnsAsync(new User{ForgetPasswordCode = "forget"});

        // Act
        var exception = await Assert.ThrowsAsync<BadArgumentException>(() => 
            _authServices.ResetPassword(resetPasswordDto)
        );
        
        // Assert
        Assert.Equal("invalid code", exception.Message);
        }
    

    private string GenerateTestToken(string username)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key"));
        // var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            // issuer: "your_issuer",
            // audience: "your_audience",
            claims: new[] { new Claim(JwtRegisteredClaimNames.Sub, username) }
            // expires: DateTime.Now.AddMinutes(Convert.ToInt32(DateTime.UtcNow.AddHours(1))),
            // expires: DateTime.UtcNow.AddHours(1),
            // signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


// dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info 
}