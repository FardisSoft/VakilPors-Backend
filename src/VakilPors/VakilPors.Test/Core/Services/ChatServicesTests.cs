using System.Collections;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MockQueryable.Moq;
using VakilPors.Contracts.Repositories;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;
using VakilPors.Core.Services;
using Xunit;

namespace VakilPors.Test.Core.Services;

public class ChatServicesTests
{
    Mock<IAppUnitOfWork> appUnitOfWorkMock;
    Mock<IPremiumService> premiumServiceMock;
    IChatServices chatServices;

    public ChatServicesTests()
    {
        appUnitOfWorkMock = new Mock<IAppUnitOfWork>();
        premiumServiceMock = new Mock<IPremiumService>();
        chatServices = new ChatServices(appUnitOfWorkMock.Object, premiumServiceMock.Object);
    }

    [Fact]
    public async Task CreateChat_ValidUsers_ReturnsExistingChat()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = 2;

        var user1 = new User { Id = userId1 };
        var user2 = new User { Id = userId2 };
        var chatMock = new Mock<Chat>();
        chatMock.SetupGet(x => x.Users).Returns(new List<User>() { user1, user2 });
        var chat = chatMock.Object;
        var chats = new List<Chat>()
        {
            chat
        };
        var chatsMock = chats.BuildMock();
        var chatRepositoryMock = new Mock<IGenericRepo<Chat>>();
        appUnitOfWorkMock.Setup(uow => uow.ChatRepo).Returns(chatRepositoryMock.Object);

        var userRepositoryMock = new Mock<IGenericRepo<User>>();
        // userRepositoryMock.Setup(repo => repo.FindAsync(It.IsAny<int>()))
        //     .ReturnsAsync((int userId) => new User { Id = userId });
        appUnitOfWorkMock.Setup(uow => uow.UserRepo).Returns(userRepositoryMock.Object);

        var lawyerRepositoryMock = new Mock<IGenericRepo<Lawyer>>();
        appUnitOfWorkMock.Setup(uow => uow.LawyerRepo).Returns(lawyerRepositoryMock.Object);

        chatRepositoryMock.Setup(repo => repo.AsQueryableNoTracking()).Returns(chatsMock);

        // Act
        var result = await chatServices.CreateChat(userId1, userId2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Users.Count);
        Assert.Contains(user1, result.Users);
        Assert.Contains(user2, result.Users);
        Assert.Equal(result, chat);

        chatRepositoryMock.Verify(repo => repo.AsQueryableNoTracking(), Times.Once());
        chatRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Chat>()), Times.Never);
        appUnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);

        lawyerRepositoryMock.Verify(repo => repo.Update(It.IsAny<Lawyer>()), Times.Never);
        appUnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateChat_EqualUserIds_ThrowsBadArgumentException()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = 1;
        // Act & Assert
        await Assert.ThrowsAsync<BadArgumentException>(() => chatServices.CreateChat(userId1, userId2));
    }

    [Fact]
    public async Task CreateChat_ChatDoesNotExist_ReturnsCreatedChat()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = 2;

        var user1 = new User { Id = userId1 };
        var user2 = new User { Id = userId2 };
        var chatMock = new Mock<Chat>();
        var chat = new Chat()
        {
            Users = new List<User>()
            {
                user1,
                user2
            },
        };
        var chats = new List<Chat>()
        {
        };
        var chatsMock = chats.BuildMock();

        var chatRepositoryMock = new Mock<IGenericRepo<Chat>>();
        appUnitOfWorkMock.Setup(uow => uow.ChatRepo).Returns(chatRepositoryMock.Object);
        chatRepositoryMock.Setup(repo => repo.AsQueryableNoTracking()).Returns(chatsMock);
        var lawyers = new List<Lawyer>()
        {
        };
        var lawyersMock = lawyers.BuildMock();

        var userRepositoryMock = new Mock<IGenericRepo<User>>();
        userRepositoryMock.Setup(repo => repo.FindAsync(userId1)).ReturnsAsync(user1);
        userRepositoryMock.Setup(repo => repo.FindAsync(userId2)).ReturnsAsync(user2);
        appUnitOfWorkMock.Setup(uow => uow.UserRepo).Returns(userRepositoryMock.Object);

        var lawyerRepositoryMock = new Mock<IGenericRepo<Lawyer>>();
        appUnitOfWorkMock.Setup(uow => uow.LawyerRepo).Returns(lawyerRepositoryMock.Object);
        lawyerRepositoryMock.Setup(repo => repo.AsQueryable()).Returns(lawyersMock);

        // Act
        var result = await chatServices.CreateChat(userId1, userId2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Users.Count);
        Assert.Contains(user1, result.Users);
        Assert.Contains(user2, result.Users);
        Assert.Equal(result.Id, chat.Id);
        Assert.Equal(result.ChatMessages, chat.ChatMessages);
        Assert.Equal(result.IsDeleted, chat.IsDeleted);

        chatRepositoryMock.Verify(repo => repo.AsQueryableNoTracking(), Times.Once);
        chatRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Chat>()), Times.Once);
        appUnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);

        lawyerRepositoryMock.Verify(repo => repo.Update(It.IsAny<Lawyer>()), Times.Never);
    }

    [Fact]
    public async Task CreateChat_ChatDoesNotExistAndUsersAreLawyers_ReturnsCreatedChatAndIncrementLawyersTokens()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = 2;

        var user1 = new User { Id = userId1 };
        var user2 = new User { Id = userId2 };
        var chatMock = new Mock<Chat>();
        var chat = new Chat()
        {
            Users = new List<User>()
            {
                user1,
                user2
            },
        };
        var chats = new List<Chat>()
        {
        };
        var chatsMock = chats.BuildMock();

        var chatRepositoryMock = new Mock<IGenericRepo<Chat>>();
        appUnitOfWorkMock.Setup(uow => uow.ChatRepo).Returns(chatRepositoryMock.Object);
        chatRepositoryMock.Setup(repo => repo.AsQueryableNoTracking()).Returns(chatsMock);
        var lawyer1 = new Lawyer()
        {
            UserId = user1.Id,
            Tokens = 2
        };
        var lawyer2 = new Lawyer()
        {
            UserId = user2.Id,
            Tokens = 3
        };
        var lawyers = new List<Lawyer>()
        {
            lawyer1,
            lawyer2
        };
        var lawyersMock = lawyers.BuildMock();

        var userRepositoryMock = new Mock<IGenericRepo<User>>();
        userRepositoryMock.Setup(repo => repo.FindAsync(userId1)).ReturnsAsync(user1);
        userRepositoryMock.Setup(repo => repo.FindAsync(userId2)).ReturnsAsync(user2);
        appUnitOfWorkMock.Setup(uow => uow.UserRepo).Returns(userRepositoryMock.Object);

        var lawyerRepositoryMock = new Mock<IGenericRepo<Lawyer>>();
        appUnitOfWorkMock.Setup(uow => uow.LawyerRepo).Returns(lawyerRepositoryMock.Object);
        lawyerRepositoryMock.Setup(repo => repo.AsQueryable()).Returns(lawyersMock);

        // Act
        var result = await chatServices.CreateChat(userId1, userId2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Users.Count);
        Assert.Contains(user1, result.Users);
        Assert.Contains(user2, result.Users);
        Assert.Equal(chat.Id,result.Id);
        Assert.Equal(chat.ChatMessages,result.ChatMessages);
        Assert.Equal(chat.IsDeleted,result.IsDeleted);
        Assert.Equal(4,lawyer1.Tokens);
        Assert.Equal(5,lawyer2.Tokens);
        chatRepositoryMock.Verify(repo => repo.AsQueryableNoTracking(), Times.Once);
        chatRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Chat>()), Times.Once);
        appUnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Exactly(3));

        lawyerRepositoryMock.Verify(repo => repo.Update(It.IsAny<Lawyer>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetChatsOfUser_WithPremiumUser_ReturnsChatsOrderedBySubscription()
    {
        // Arrange
        var userId = 1;

        var user = new User { Id = userId };

        var chat1 = new Chat { Id = 1, Users = new List<User> { user, new User { Id = 2 } } };
        var chat2 = new Chat { Id = 2, Users = new List<User> { user, new User { Id = 3 } } };
        var chat3 = new Chat { Id = 3, Users = new List<User> { user, new User { Id = 4 } } };

        var chatList = new List<Chat> { chat1, chat2, chat3 };

        var chatRepositoryMock = new Mock<IGenericRepo<Chat>>();
        chatRepositoryMock.Setup(repo => repo.AsQueryableNoTracking()).Returns(chatList.BuildMock());
        appUnitOfWorkMock.Setup(uow => uow.ChatRepo).Returns(chatRepositoryMock.Object);

        premiumServiceMock.Setup(service => service.DoseUserHaveAnyActiveSubscription(It.IsAny<int>())).ReturnsAsync(true);

        // Act
        var result = await chatServices.GetChatsOfUser(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        // Assert that the result is ordered by subscription status
        Assert.Equal(chat1, result.ElementAt(0));
        Assert.Equal(chat2, result.ElementAt(1));
        Assert.Equal(chat3, result.ElementAt(2));

        // Verify that the premium service method was called for each user (except the requesting user)
        premiumServiceMock.Verify(service => service.DoseUserHaveAnyActiveSubscription(It.IsAny<int>()), Times.Exactly(3));
        premiumServiceMock.Verify(service => service.DoseUserHaveAnyActiveSubscription(2), Times.Once);
        premiumServiceMock.Verify(service => service.DoseUserHaveAnyActiveSubscription(3), Times.Once);
        premiumServiceMock.Verify(service => service.DoseUserHaveAnyActiveSubscription(4), Times.Once);
    }
    [Fact]
    public async Task GetChatsWithMessagesOfUser_WithPremiumUser_ReturnsChatsOrderedBySubscription()
    {
        // Arrange
        var userId = 1;

        var user = new User { Id = userId };

        var chat1 = new Chat { Id = 1, Users = new List<User> { user, new User { Id = 2 } } };
        var chat2 = new Chat { Id = 2, Users = new List<User> { user, new User { Id = 3 } } };
        var chat3 = new Chat { Id = 3, Users = new List<User> { user, new User { Id = 4 } } };

        var chatMessage1 = new ChatMessage { Id = 1, Sender = user, Chat = chat1 };
        var chatMessage2 = new ChatMessage { Id = 2, Sender = user, Chat = chat2 };
        var chatMessage3 = new ChatMessage { Id = 3, Sender = user, Chat = chat3 };

        chat1.ChatMessages = new List<ChatMessage> { chatMessage1 };
        chat2.ChatMessages = new List<ChatMessage> { chatMessage2 };
        chat3.ChatMessages = new List<ChatMessage> { chatMessage3 };

        var chatList = new List<Chat> { chat1, chat2, chat3 };

        var chatRepositoryMock = new Mock<IGenericRepo<Chat>>();
        chatRepositoryMock.Setup(repo => repo.AsQueryableNoTracking()).Returns(chatList.BuildMock());
        appUnitOfWorkMock.Setup(uow => uow.ChatRepo).Returns(chatRepositoryMock.Object);

        premiumServiceMock.Setup(service => service.DoseUserHaveAnyActiveSubscription(It.IsAny<int>())).ReturnsAsync(true);

        // Act
        var result = await chatServices.GetChatsWithMessagesOfUser(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        // Assert that the result is ordered by subscription status
        Assert.Equal(chat1, result.ElementAt(0));
        Assert.Equal(chat2, result.ElementAt(1));
        Assert.Equal(chat3, result.ElementAt(2));

        // Verify that the premium service method was called for each user (except the requesting user)
        premiumServiceMock.Verify(service => service.DoseUserHaveAnyActiveSubscription(It.IsAny<int>()), Times.Exactly(3));
        premiumServiceMock.Verify(service => service.DoseUserHaveAnyActiveSubscription(2), Times.Once);
        premiumServiceMock.Verify(service => service.DoseUserHaveAnyActiveSubscription(3), Times.Once);
        premiumServiceMock.Verify(service => service.DoseUserHaveAnyActiveSubscription(4), Times.Once);
    }
    [Fact]
    public async Task GetMessagesOfChat_ValidUserAndChat_ReturnsChatMessages()
    {
        // Arrange
        var userId = 1;
        var chatId = 1;
        var user = new User { Id = userId };

        var chat = new Chat { Id = chatId, Users = new List<User> { user, new User { Id = 2 } } };

        var chatMessage1 = new ChatMessage { Id = 1, Sender = user, Chat = chat };
        var chatMessage2 = new ChatMessage { Id = 2, Sender = user, Chat = chat };

        chat.ChatMessages = new List<ChatMessage> { chatMessage1, chatMessage2 };

        var chatRepositoryMock = new Mock<IGenericRepo<Chat>>();
        chatRepositoryMock.Setup(repo => repo.AsQueryableNoTracking()).Returns(new List<Chat> { chat }.BuildMock());
        appUnitOfWorkMock.Setup(uow => uow.ChatRepo).Returns(chatRepositoryMock.Object);

        // Act
        var result = await chatServices.GetMessagesOfChat(userId, chatId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        // Verify that the result contains the expected chat messages
        Assert.Contains(chatMessage1, result);
        Assert.Contains(chatMessage2, result);

        // Verify that the exception is not thrown for a valid user
        Assert.Null(await Record.ExceptionAsync(() => chatServices.GetMessagesOfChat(userId, chatId)));
    }

    [Fact]
    public async Task GetMessagesOfChat_InvalidUser_ThrowsAccessViolationException()
    {
        // Arrange
        var userId = 1;
        var chatId = 1;
        var user = new User { Id = userId };

        var chat = new Chat { Id = chatId, Users = new List<User> { new User { Id = 2 } } };

        var chatRepositoryMock = new Mock<IGenericRepo<Chat>>();
        chatRepositoryMock.Setup(repo => repo.AsQueryableNoTracking()).Returns(new List<Chat> { chat }.BuildMock());
        appUnitOfWorkMock.Setup(uow => uow.ChatRepo).Returns(chatRepositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<AccessViolationException>(() => chatServices.GetMessagesOfChat(userId, chatId));

        // Verify that the exception is thrown for an invalid user
        Assert.NotNull(await Record.ExceptionAsync(() => chatServices.GetMessagesOfChat(userId, chatId)));
    }
}