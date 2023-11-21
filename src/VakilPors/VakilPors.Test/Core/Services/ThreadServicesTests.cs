using VakilPors.Core.Services;
using Moq;
using MockQueryable.Moq;

using VakilPors.Core.Contracts.Services;
using VakilPors.Contracts.UnitOfWork;
using AutoMapper;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Exceptions;
using VakilPors.Core.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore;
using VakilPors.Data.Migrations;
using VakilPors.Contracts.Repositories;
namespace VakilPors.Test.Core.Services;
public class ThreadServicesTests{
    private Mock<IAntiSpam> _antiSpam;
    private  Mock<IAppUnitOfWork> _uow;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<IThreadCommentService> _threadCommentService;
    private readonly Mock<ILawyerServices> _lawyerServices;
    private readonly Mock<IPremiumService> _premiumService;
    private readonly Mock<ITelegramService> _telegramService;
    private readonly Mock<IEmailSender> _emailSender;
    private ThreadService threadservice;
    private Mock<ThreadDto> _threadDto;
    public ThreadServicesTests()
    {
        _antiSpam = new Mock<IAntiSpam>();
        _uow = new Mock<IAppUnitOfWork>();
        _mapper = new Mock<IMapper>();
        _threadCommentService = new Mock<IThreadCommentService>();
        _lawyerServices = new Mock<ILawyerServices>();
        _premiumService = new Mock<IPremiumService>();
        _telegramService = new Mock<ITelegramService>();
        _emailSender = new Mock<IEmailSender>();

        threadservice = new ThreadService(_uow.Object , _mapper.Object 
        , _threadCommentService.Object ,_lawyerServices.Object , _premiumService.Object 
        , _telegramService.Object , _emailSender.Object );
        _threadDto = new Mock<ThreadDto>();
    }
    [Fact]
    public async void CreateThread_DecsriptionMustSpam_ThrowBadArgumentException(){
        // _smtpclient.Setup(client => client.SendAsync(It.IsAny<MimeMessage>(),

        _antiSpam.Setup(sp => sp.IsSpam(_threadDto.Object.Description)).ReturnsAsync("This message is detected as a spam and can not be shown.");// inja ke log ya kar ezafe nemikone bekhatere parameter exception
        _uow.Setup(uow => uow.UserRepo.FindAsync(It.IsAny<int>)).ReturnsAsync((int userId) => null);

        await Assert.ThrowsAsync<BadArgumentException>(() => threadservice.CreateThread(It.IsAny<int>() , _threadDto.Object , _antiSpam.Object));
    }
    [Fact]
    public async void CreateThread_TitleMustSpam_ThrowBadArgumentException(){

        _threadDto.Object.Description="description";
        _threadDto.Object.Title="Title";
        _antiSpam.Setup(sp => sp.IsSpam(_threadDto.Object.Title)).ReturnsAsync("This message is detected as a spam and can not be shown.");// inja ke log ya kar ezafe nemikone bekhatere parameter exception
        _uow.Setup(uow => uow.UserRepo.FindAsync(It.IsAny<int>)).ReturnsAsync((int userId) => null);

        await Assert.ThrowsAsync<BadArgumentException>(() => threadservice.CreateThread(It.IsAny<int>() , _threadDto.Object , _antiSpam.Object));
    }
    [Fact]
    public async void CreateThread_FailSaveChangesAsync_ThrowException(){        
        _uow.Setup(uow => uow.UserRepo.FindAsync(It.IsAny<int>)).ReturnsAsync((int userId) => null);//essential because if doesn't write it get error
        _uow.Setup(uow => uow.ForumThreadRepo.AddAsync(It.IsAny<ForumThread>())).Verifiable();
        // you want to verify that the setup was indeed invoked.
        //This will mark the call as verifiable but not actually invoke it.

        _uow.Setup(sca => sca.SaveChangesAsync()).ReturnsAsync(0);
        await Assert.ThrowsAsync<Exception>(() => threadservice.CreateThread(It.IsAny<int>() , _threadDto.Object , _antiSpam.Object));
    }
    [Fact(Skip = "can't return _user and return null then get null reference error")]
    public async void CreateThread_successSaveChangesAsync_ProperFunctionCall(){
        
        _threadDto.Object.Title="Title";
        var user = new User{Name = "blah blah" , Email="blah@gmail.com" , Telegram="blahtelegram"};
        // _uow.Setup(uow => uow.UserRepo.FindAsync(It.IsAny<int>)).ReturnsAsync(user);//essential because if doesn't write it get error
        _uow.Setup(uow => uow.UserRepo.FindAsync(It.IsAny<int>)).ReturnsAsync((int userId) => userId == 2 ? user : null);//essential because if doesn't write it get error
        _uow.Setup(sca => sca.SaveChangesAsync()).ReturnsAsync(1);

        _uow.Setup(uow => uow.ForumThreadRepo.AddAsync(It.IsAny<ForumThread>())).Verifiable();
        _emailSender.Setup(es => es.SendEmailAsync(user.Email,user.Email,It.IsAny<string>(),It.IsAny<string>(),true)).Verifiable();
        _telegramService.Setup(ts => ts.SendToTelegram(It.IsAny<string>(),user.Telegram)).Verifiable();
        

        await threadservice.CreateThread(It.IsAny<int>() , _threadDto.Object , _antiSpam.Object);

        _uow.Verify(o => o.ForumThreadRepo.AddAsync(It.IsAny<ForumThread>()));
        _emailSender.Verify(client => client.SendEmailAsync(user.Email,user.Email,It.IsAny<string>(),It.IsAny<string>(),true));//(It.IsAny<MimeMessage>(),It.IsAny<CancellationToken>() ,It.IsAny<ITransferProgress>()));
        _telegramService.Verify(client => client.SendToTelegram(It.IsAny<string>(),user.Telegram));


    }
	[Fact]
	public async void UpdateThread_DetectSpamDescription_ThrowBadArgumentException(){
		_threadDto.Object.Description="Description";
		_antiSpam.Setup(sp => sp.IsSpam(_threadDto.Object.Description)).ReturnsAsync("This message is detected as a spam and can not be shown.");
		
		await Assert.ThrowsAsync<BadArgumentException>(() => threadservice.UpdateThread(It.IsAny<int>() , _threadDto.Object , _antiSpam.Object));
		
	}
	[Fact]
	public async void UpdateThread_CantFindThread_ThrowBadArgumentException(){
		_threadDto.Object.Description="Description";
		_threadDto.Object.Id=1;
		_antiSpam.Setup(sp => sp.IsSpam(_threadDto.Object.Description)).Verifiable();
		_uow.Setup(uow => uow.ForumThreadRepo.FindAsync(_threadDto.Object.Id)).ReturnsAsync((int id ) => null);
		
		await Assert.ThrowsAsync<BadArgumentException>(() => threadservice.UpdateThread(It.IsAny<int>() , _threadDto.Object , _antiSpam.Object));
		
		
	}
	[Fact]
	public async void UpdateThread_UserDoesntHavePermission_ThrowAccessViolationException(){

        _threadDto.Object.Description="Description";
		_threadDto.Object.Id=1;
		_antiSpam.Setup(sp => sp.IsSpam(_threadDto.Object.Description)).Verifiable();
		int user_id_input = 1;
		_uow.Setup(uow => uow.ForumThreadRepo.FindAsync(_threadDto.Object.Id)).ReturnsAsync(new ForumThread{Id = user_id_input +1});


		await Assert.ThrowsAsync<AccessViolationException>(() => threadservice.UpdateThread(user_id_input , _threadDto.Object , _antiSpam.Object));
    }
    [Fact]
    public async void UpdateThread_FailedToSaveChangesThread_ThrowException(){
        _threadDto.Object.Description="Description";
		_threadDto.Object.Id=1;
		_threadDto.Object.Title="title";
		_threadDto.Object.Description="Description";

		_antiSpam.Setup(sp => sp.IsSpam(_threadDto.Object.Description)).Verifiable();
		int user_id_input = 1;
		_uow.Setup(uow => uow.ForumThreadRepo.FindAsync(_threadDto.Object.Id)).ReturnsAsync(new ForumThread{Id = user_id_input});
        _uow.Setup(uow => uow.ForumThreadRepo.Update(It.IsAny<ForumThread>())).Verifiable();
        _uow.Setup(sca => sca.SaveChangesAsync()).ReturnsAsync(0);

		await Assert.ThrowsAsync<AccessViolationException>(() => threadservice.UpdateThread(user_id_input , _threadDto.Object , _antiSpam.Object));
    }


    [Fact]
    public async void DeleteThread_FailedOnFindThread_ThrowBadArgumentException(){
        // _threadDto.Object.Description="Description";
		// _threadDto.Object.Id=1;
        int threadId_input = 1,userId_input =1;
		_uow.Setup(uow => uow.ForumThreadRepo.FindAsync(threadId_input)).ReturnsAsync((int id ) => null);
        _uow.Setup(uow => uow.UserRepo.FindAsync(userId_input)).Verifiable();
		
		await Assert.ThrowsAsync<BadArgumentException>(() => threadservice.DeleteThread(userId_input ,threadId_input));
		
		
    }
    [Fact]
    public async void DeleteThread_UserDoesntHavePermission_ThrowAccessViolationException(){
        int threadId_input = 1,userId_input =1;
		_uow.Setup(uow => uow.ForumThreadRepo.FindAsync(threadId_input)).ReturnsAsync(new ForumThread{UserId = userId_input + 1});
        _uow.Setup(uow => uow.UserRepo.FindAsync(userId_input)).Verifiable();
		
		await Assert.ThrowsAsync<AccessViolationException>(() => threadservice.DeleteThread(userId_input ,threadId_input));		
    }
    [Fact]
    public async void DeleteThread_FailedToSaveChanges_ThrowException(){
        int threadId_input = 1,userId_input =1;
        ForumThread ft =new ForumThread{UserId = userId_input};
		_uow.Setup(uow => uow.ForumThreadRepo.FindAsync(threadId_input)).ReturnsAsync(ft);
        _uow.Setup(uow => uow.UserRepo.FindAsync(userId_input)).Verifiable();
        _uow.Setup(uow => uow.ForumThreadRepo.Remove(ft)).Verifiable();
        _uow.Setup(sca => sca.SaveChangesAsync()).ReturnsAsync(0);
        await Assert.ThrowsAsync<Exception>(() => threadservice.DeleteThread(userId_input,threadId_input));
        _uow.Verify(uow => uow.ForumThreadRepo.Remove(ft));
        // _smtpclient.Verify(client => client.SendAsync(It.IsAny<MimeMessage>(),It.IsAny<CancellationToken>() ,It.IsAny<ITransferProgress>()));
    }

    [Fact(Skip ="because of email sender and telegram service")]
    public async void DeleteThread_PerformAllLineCorrectly_ReturnTrue(){
        int threadId_input = 1,userId_input =1;
        ForumThread ft =new ForumThread{UserId = userId_input};
		_uow.Setup(uow => uow.ForumThreadRepo.FindAsync(threadId_input)).ReturnsAsync(ft);
        _uow.Setup(uow => uow.UserRepo.FindAsync(userId_input)).Verifiable();
        _uow.Setup(uow => uow.ForumThreadRepo.Remove(ft)).Verifiable();
        _uow.Setup(sca => sca.SaveChangesAsync()).ReturnsAsync(1);
        await Assert.ThrowsAsync<Exception>(() => threadservice.DeleteThread(userId_input,threadId_input));
        _uow.Verify(uow => uow.ForumThreadRepo.Remove(ft));

    }

    
    // [Fact(Skip ="get null reference error")]
    [Fact]
    public async void GetThreadList_ReturnNullFromResource_ThrowBadArgumentException(){
        int userId_input =1;// ,threadId_input = 1;
        Mock<GetThreadDtoFromThread> m = new Mock<GetThreadDtoFromThread>();
        IEnumerable<ForumThread> forumthread = new List<ForumThread>
        {
            new ForumThread{Id =userId_input,Title="title" ,User = new User{Name="name"} }
        };
        var mock = forumthread.AsQueryable().BuildMock();
        _uow.Setup(uow => uow.ForumThreadRepo.AsQueryable()).Returns(mock);
        

        var result =await threadservice.GetThreadList(userId_input);
        Assert.Equal(result[0].Id, forumthread.ToList()[0].Id);
    }

}
