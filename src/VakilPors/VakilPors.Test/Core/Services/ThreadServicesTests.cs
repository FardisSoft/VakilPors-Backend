using VakilPors.Core.Services;
using Moq;
using MockQueryable.Moq;
using AutoFixture;

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
    private Mock<IGenericRepo<User>> _userRepoMock;
    private Mock<IGenericRepo<ForumThread>> _forumThreadRepo;
    private User _user;

    public ThreadServicesTests()
    {
        _user = new User{Email = "email" , Name = "name" , Telegram="telegram"};
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

        _userRepoMock= new Mock<IGenericRepo<User>>();
        _forumThreadRepo = new Mock<IGenericRepo<ForumThread>>();
        _uow.Setup(uow => uow.UserRepo).Returns(_userRepoMock.Object);
        _uow.Setup(uow => uow.ForumThreadRepo).Returns(_forumThreadRepo.Object);

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
    // [Fact(Skip = "first must complete GetThreadWithComments function")]
    [Fact]
    public async void CreateThread_successSaveChangesAsync_ProperFunctionCall(){
        //assert for GetThreadWithComments function
        var fixture = new Fixture();
            //AutoFixture
        // var thread = fixture.Build < ForumThread > ().With(a => a.FirstName, firstName).With(a => a.LastName, lastName).Create();
        ForumThread thread = fixture.Build<ForumThread>().Without(p => p.User)
        .Without( X=>X.UserLikes).Create();
        thread.Id=0;
        thread.User= new User{ Name = "name"};
        thread.UserLikes = new List<UserThreadLike>
        {
            new UserThreadLike{UserId = 1}
        };

        var u = new List<ForumThread>{thread};
        var submock =u.BuildMock();
        _forumThreadRepo.Setup(urm => urm.AsQueryable()).Returns(submock);
        //-----
        _threadDto.Object.Title="Title";
        var user = new User{Name = "blah blah" , Email="blah@gmail.com" , Telegram="blahtelegram"};

        // var u = new List<User>{ _user };
        // var submock =u.BuildMock();
        _userRepoMock.Setup(urm => urm.FindAsync(It.IsAny<int>())).ReturnsAsync(_user);

        _uow.Setup(sca => sca.SaveChangesAsync()).ReturnsAsync(1);

        _uow.Setup(uow => uow.ForumThreadRepo.AddAsync(It.IsAny<ForumThread>())).Verifiable();
        _emailSender.Setup(es => es.SendEmailAsync(It.IsAny<string>(),It.IsAny<string>() ,It.IsAny<string>(),It.IsAny<string>(),true)).Returns(Task.CompletedTask);
        _telegramService.Setup(ts => ts.SendToTelegram(It.IsAny<string>(),user.Telegram)).Returns(Task.CompletedTask);
        

        await threadservice.CreateThread(It.IsAny<int>() , _threadDto.Object , _antiSpam.Object);

        _uow.Verify(o => o.ForumThreadRepo.AddAsync(It.IsAny<ForumThread>()));
        _emailSender.Verify(client => client.SendEmailAsync(It.IsAny<string>(),It.IsAny<string>(),It.IsAny<string>(),It.IsAny<string>(),true),Times.Once);//(It.IsAny<MimeMessage>(),It.IsAny<CancellationToken>() ,It.IsAny<ITransferProgress>()));
        _telegramService.Verify(client => client.SendToTelegram(It.IsAny<string>(),It.IsAny<string>()),Times.Once);


    }
    [Fact]
    public async void GetThreadWithComments_AsQueryableReturnNull_ThrowBadArgumentException(){

        var u = new List<ForumThread>{};
        var submock =u.BuildMock();
        _forumThreadRepo.Setup(urm => urm.AsQueryable()).Returns(submock);

        // await threadservice.GetThreadWithComments(It.IsAny<int>() , It.IsAny<int>() );
        await Assert.ThrowsAsync<BadArgumentException>(() => threadservice.GetThreadWithComments(It.IsAny<int>() , It.IsAny<int>() ));
    }
    [Fact]
    public async void GetThreadWithComments_ReturnTrueThread_SuccessWay(){

        var fixture = new Fixture();
            //AutoFixture
        // var thread = fixture.Build < ForumThread > ().With(a => a.FirstName, firstName).With(a => a.LastName, lastName).Create();
        ForumThread thread = fixture.Build<ForumThread>().Without(p => p.User)
        .Without( X=>X.UserLikes).Create();
        thread.Id=0;
        thread.User= new User{ Name = "name"};
        thread.UserLikes = new List<UserThreadLike>
        {
            new UserThreadLike{UserId = 1}
        };

        var u = new List<ForumThread>{thread};
        var submock =u.BuildMock();
        _forumThreadRepo.Setup(urm => urm.AsQueryable()).Returns(submock);

        var result = await threadservice.GetThreadWithComments(It.IsAny<int>() , It.IsAny<int>() );
        // await Assert.ThrowsAsync<BadArgumentException>(() => threadservice.GetThreadWithComments(It.IsAny<int>() , It.IsAny<int>() ));
        Assert.Equal(result.GetType() ,typeof(ThreadWithCommentsDto) );
        Assert.NotNull(result);
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
		_uow.Setup(uow => uow.ForumThreadRepo.FindAsync(_threadDto.Object.Id)).ReturnsAsync(new ForumThread{UserId = user_id_input});
        _uow.Setup(uow => uow.ForumThreadRepo.Update(It.IsAny<ForumThread>())).Verifiable();
        _uow.Setup(sca => sca.SaveChangesAsync()).ReturnsAsync(0);

		await Assert.ThrowsAsync<Exception>(() => threadservice.UpdateThread(user_id_input , _threadDto.Object , _antiSpam.Object));
    }
    [Fact]
    public async void UpdateThread_UpdateThread_ReturnThreadDto(){
        //GetThreadWithComments
        var fixture = new Fixture();
            //AutoFixture
        // var thread = fixture.Build < ForumThread > ().With(a => a.FirstName, firstName).With(a => a.LastName, lastName).Create();
        ForumThread thread = fixture.Build<ForumThread>().Without(p => p.User)
        .Without( X=>X.UserLikes).Create();
        thread.Id=0;
        thread.User= new User{ Name = "name"};
        thread.UserLikes = new List<UserThreadLike>
        {
            new UserThreadLike{UserId = 1}
        };

        var u = new List<ForumThread>{thread};
        var submock =u.BuildMock();
        _forumThreadRepo.Setup(urm => urm.AsQueryable()).Returns(submock);
        //--------------
        _threadDto.Object.Description="Description";
		_threadDto.Object.Id=1;
		_threadDto.Object.Title="title";
		_threadDto.Object.Description="Description";

		_antiSpam.Setup(sp => sp.IsSpam(_threadDto.Object.Description)).Verifiable();
		int user_id_input = 1;
		_uow.Setup(uow => uow.ForumThreadRepo.FindAsync(_threadDto.Object.Id)).ReturnsAsync(new ForumThread{UserId = user_id_input});
        _uow.Setup(uow => uow.ForumThreadRepo.Update(It.IsAny<ForumThread>())).Verifiable();
        _uow.Setup(sca => sca.SaveChangesAsync()).ReturnsAsync(1);

		ThreadDto result = await threadservice.UpdateThread(user_id_input , _threadDto.Object , _antiSpam.Object);

        Assert.Equal(typeof(ThreadDto) , result.GetType());


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

    // [Fact(Skip ="because of email sender and telegram service")]
    [Fact]
    public async void DeleteThread_PerformAllLineCorrectly_ReturnTrue(){
        int threadId_input = 1,userId_input =1;
        ForumThread ft =new ForumThread{UserId = userId_input};
		_uow.Setup(uow => uow.ForumThreadRepo.FindAsync(threadId_input)).ReturnsAsync(ft);
        _uow.Setup(uow => uow.UserRepo.FindAsync(userId_input)).ReturnsAsync(_user);
        _uow.Setup(uow => uow.ForumThreadRepo.Remove(ft)).Verifiable();
        _uow.Setup(sca => sca.SaveChangesAsync()).ReturnsAsync(1);
        var result = await threadservice.DeleteThread(userId_input , threadId_input);
        // await Assert.ThrowsAsync<Exception>(() => threadservice.DeleteThread(userId_input,threadId_input));
        Assert.True(result);
        _uow.Verify(uow => uow.ForumThreadRepo.Remove(ft));

    }

    
    // [Fact(Skip ="get null reference error")]
    [Theory]
    [InlineData(2)]   // Test case 1: 2 + 3 = 5
    [InlineData(1)]   // Test case 1: 2 + 3 = 5
    [InlineData(0)]   // Test case 1: 2 + 3 = 5
    public async void GetThreadList_ReturntwoThread_ReturnCorrectNumberOfThread(int thread_count){
        int userId_input =1;// ,threadId_input = 1;
        // Mock<GetThreadDtoFromThread> m = new Mock<GetThreadDtoFromThread>();
        IEnumerable<ForumThread> forumthread = new List<ForumThread>{};
        for (int i = 0; i < thread_count; i++)
        {
            forumthread = forumthread.Append
            (
            new ForumThread
                {
                    Id =userId_input+i,Title="title" ,User = new User{Name="name"} ,
                    UserLikes = new List<UserThreadLike>
                    {
                        new UserThreadLike{UserId = 1}
                    }
                }
            );
        }
        
        var mock = forumthread.AsQueryable().BuildMock();
        _uow.Setup(uow => uow.ForumThreadRepo.AsQueryable()).Returns(mock);
        

        var result =await threadservice.GetThreadList(userId_input);
        for (int i = 0; i < thread_count; i++)
        {
            Assert.Equal(result[i].Id, forumthread.ToList()[i].Id);            
        }
        // Assert.Equal(result[1].Id, forumthread.ToList()[1].Id);
        Assert.Equal(thread_count,result.Count());
    }

    [Fact]
    public async Task LikeThread_UserSuccessfullyLiked_LikeCountIncrease()
    {
        // Arrange
        var userId = 1;
        var threadId = 1;
        var initialLikeCount = 0;

        // var mockSet = new Mock<DbSet<ForumThread>>();
        var threads = new List<ForumThread>
        {
            new ForumThread { Id = threadId, LikeCount = initialLikeCount, UserLikes = new List<UserThreadLike>() }
        }.AsQueryable();
        var threadmock = threads.AsQueryable().BuildMock();
        _uow.Setup(uow => uow.ForumThreadRepo.AsQueryable()).Returns(threadmock);

        _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1); // simulating a successful save

        // Act
        var result =await threadservice.LikeThread(userId , threadId);

        // Assert
        Assert.Equal(initialLikeCount + 1, result);
    }
    [Fact]
    public async Task LikeThread_CantFindThread_ThrowBadArgumentException()
    {
        // Arrange
        var userId = 1;
        var threadId = 1;
        // var initialLikeCount = 0;

        // var mockSet = new Mock<DbSet<ForumThread>>();
        var threads = new List<ForumThread>
        {
            // new ForumThread { Id = threadId, LikeCount = initialLikeCount, UserLikes = new List<UserThreadLike>() }
        };//.AsQueryable();
        var threadmock = threads.AsQueryable().BuildMock();
        _uow.Setup(uow => uow.ForumThreadRepo.AsQueryable()).Returns(threadmock);

        // _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1); // simulating a successful save

        // Act
        await Assert.ThrowsAsync<BadArgumentException>(() => threadservice.LikeThread(userId , threadId));
        }
    [Fact]
    public async Task LikeThread_FindLike_ReturnLikeCount()
    {
        // Arrange
        var userId = 1;
        var threadId = 1;
        var initialLikeCount = 0;

        // var mockSet = new Mock<DbSet<ForumThread>>();
        var threads = new List<ForumThread>
        {
            new ForumThread { Id = threadId, LikeCount = initialLikeCount,
            UserLikes = new List<UserThreadLike>{ new UserThreadLike{ UserId=userId} }
            }
        };//.AsQueryable();
        var threadmock = threads.AsQueryable().BuildMock();
        _uow.Setup(uow => uow.ForumThreadRepo.AsQueryable()).Returns(threadmock);

        // _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1); // simulating a successful save

        // Act
        var result = await  threadservice.LikeThread(userId , threadId);
        Assert.Equal(initialLikeCount , result);
        }
    [Fact]
    public async Task LikeThread_CantUpdate_ThrowException()
    {
        // Arrange
        var userId = 1;
        var threadId = 1;
        var initialLikeCount = 0;

        // var mockSet = new Mock<DbSet<ForumThread>>();
        var threads = new List<ForumThread>
        {
            new ForumThread { Id = threadId, LikeCount = initialLikeCount,
            UserLikes = new List<UserThreadLike>{ new UserThreadLike{ UserId=userId -1} }
            }
        };//.AsQueryable();
        var threadmock = threads.AsQueryable().BuildMock();
        _uow.Setup(uow => uow.ForumThreadRepo.AsQueryable()).Returns(threadmock);

        // _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1); // simulating a successful save
        _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(0); // simulating a successful save

        // Act
        await Assert.ThrowsAsync<Exception>(() => threadservice.LikeThread(userId , threadId));

        }

    [Fact]
    public async Task UndoLikeThread_CantFindThread_ThrowBadArgumentException()
    {
        // Arrange
        var userId = 1;
        var threadId = 1;
        // var initialLikeCount = 0;

        // var mockSet = new Mock<DbSet<ForumThread>>();
        var threads = new List<ForumThread>
        {
            // new ForumThread { Id = threadId, LikeCount = initialLikeCount, UserLikes = new List<UserThreadLike>() }
        };//.AsQueryable();
        var threadmock = threads.AsQueryable().BuildMock();
        _uow.Setup(uow => uow.ForumThreadRepo.AsQueryable()).Returns(threadmock);

        // _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1); // simulating a successful save

        // Act
        await Assert.ThrowsAsync<BadArgumentException>(() => threadservice.UndoLikeThread(userId , threadId));
    }
    [Fact]
    public async Task UndoLikeThread_FindLike_ReturnLikeCount()
    {
        // Arrange
        var userId = 1;
        var threadId = 1;
        var initialLikeCount = 1;

        // var mockSet = new Mock<DbSet<ForumThread>>();
        var threads = new List<ForumThread>
        {
            new ForumThread { Id = threadId , LikeCount = initialLikeCount,
            UserLikes = new List<UserThreadLike>{ new UserThreadLike{ UserId=userId , ThreadId = threadId } }
            }
        };//.AsQueryable();
        var threadmock = threads.AsQueryable().BuildMock();
        _uow.Setup(uow => uow.ForumThreadRepo.AsQueryable()).Returns(threadmock);

        _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1); // simulating a successful save

        // Act
        var result = await  threadservice.UndoLikeThread(userId , threadId);
        Assert.Equal(initialLikeCount-1 , result);
    }
    [Fact]
    public async Task UndoLikeThread_CantUpdate_ThrowException()
    {
        // Arrange
        var userId = 1;
        var threadId = 1;
        var initialLikeCount = 1;

        // var mockSet = new Mock<DbSet<ForumThread>>();
        var threads = new List<ForumThread>
        {
            new ForumThread { Id = threadId , LikeCount = initialLikeCount,
            UserLikes = new List<UserThreadLike>{ new UserThreadLike{ UserId=userId , ThreadId = threadId } }
            }
        };//.AsQueryable();
        var threadmock = threads.AsQueryable().BuildMock();
        _uow.Setup(uow => uow.ForumThreadRepo.AsQueryable()).Returns(threadmock);

        _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(0); // simulating a successful save

        // Act
        await Assert.ThrowsAsync<Exception>(() => threadservice.UndoLikeThread(userId , threadId));
    }

    [Fact]
    public async Task UndoLikeThread_ZeroLike_ReturnLikeCount()
    {
        // Arrange
        var userId = 1;
        var threadId = 1;
        var initialLikeCount = 0;

        // var mockSet = new Mock<DbSet<ForumThread>>();
        var threads = new List<ForumThread>
        {
            new ForumThread { Id = threadId , LikeCount = initialLikeCount,
            UserLikes = new List<UserThreadLike>{ new UserThreadLike{ UserId=userId , ThreadId = threadId } }
            }
        };//.AsQueryable();
        var threadmock = threads.AsQueryable().BuildMock();
        _uow.Setup(uow => uow.ForumThreadRepo.AsQueryable()).Returns(threadmock);

        // _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(0); // simulating a successful save

        // Act
        // await Assert.ThrowsAsync<Exception>(() => threadservice.UndoLikeThread(userId , threadId));
        var result = await threadservice.UndoLikeThread(userId , threadId);
        Assert.Equal(initialLikeCount , result);
    }

}
