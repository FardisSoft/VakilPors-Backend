using Microsoft.AspNetCore.Identity;
using Moq;
using VakilPors.Contracts.Repositories;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;
using VakilPors.Core.Services;

namespace VakilPors.Test.Core.Services;

public class WalletServicesTests
{
    private readonly WalletServices walletServices;
    private readonly Mock<UserManager<User>> userManagerMock;
    private readonly Mock<IAppUnitOfWork> appUnitOfWorkMock;
    private readonly Mock<IEmailSender> emailSenderMock;
    private readonly Mock<ITelegramService> telegramServiceMock;

    public WalletServicesTests()
    {
        userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(),
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        );

        appUnitOfWorkMock = new Mock<IAppUnitOfWork>();
        emailSenderMock = new Mock<IEmailSender>();
        telegramServiceMock = new Mock<ITelegramService>();

        walletServices = new WalletServices(
            userManagerMock.Object,
            appUnitOfWorkMock.Object,
            emailSenderMock.Object,
            telegramServiceMock.Object
        );
    }

    [Fact]
    public async Task AddBalance_ByPhoneNumber_UpdatesUserBalance()
    {
        // Arrange
        var phoneNumber = "123456789";
        var user = new User { PhoneNumber = phoneNumber };
        userManagerMock.Setup(m => m.FindByNameAsync(phoneNumber)).ReturnsAsync(user);
        userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        await walletServices.AddBalance(phoneNumber, 100);

        // Assert
        Assert.Equal(100, user.Balance);
        userManagerMock.Verify(m => m.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task AddBalance_ByUserId_UpdatesUserBalance()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId };
        userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        await walletServices.AddBalance(userId, 100);

        // Assert
        Assert.Equal(100, user.Balance);
        userManagerMock.Verify(m => m.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task AddBalance_FailsToUpdateUser_ThrowsInternalServerException()
    {
        // Arrange
        var phoneNumber = "123456789";
        var user = new User { PhoneNumber = phoneNumber };
        userManagerMock.Setup(m => m.FindByNameAsync(phoneNumber)).ReturnsAsync(user);
        userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Failed());

        // Act & Assert
        await Assert.ThrowsAsync<InternalServerException>(() => walletServices.AddBalance(phoneNumber, 100));
    }

    [Fact]
    public async Task AddTransaction_AddsTransactionAndUpdatesBalance()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId, Email = "test@example.com", Telegram = "telegram_user" };
        var transactionId = 123;
        var amount = 50m;
        var description = "Test transaction";
        var authority = "ABC123";
        var isSuccess = true;
        var isIncome = true;

        userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        appUnitOfWorkMock.Setup(m => m.TransactionRepo.AddAsync(It.IsAny<Transaction>()));
        appUnitOfWorkMock.Setup(m => m.SaveChangesAsync());
        emailSenderMock.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),It.IsAny<bool>())).Returns(Task.CompletedTask);
        telegramServiceMock.Setup(m => m.SendToTelegram(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
        // Act
        await walletServices.AddTransaction(userId, amount, description, authority, isSuccess, isIncome);

        // Assert
        appUnitOfWorkMock.Verify(m => m.TransactionRepo.AddAsync(It.IsAny<Transaction>()), Times.Once);
        appUnitOfWorkMock.Verify(m => m.SaveChangesAsync(), Times.Once);
        userManagerMock.Verify(m => m.UpdateAsync(user), Times.Once);
        emailSenderMock.Verify(m => m.SendEmailAsync(user.Email, user.Name, "تراکنش", It.IsAny<string>(),It.IsAny<bool>()), Times.Once);
        telegramServiceMock.Verify(m => m.SendToTelegram(It.IsAny<string>(), user.Telegram), Times.Once);
    }

    // ...

    [Fact]
    public async Task ApproveTransaction_MarksTransactionAsSuccessfulAndUpdatesBalance()
    {
        // Arrange
        var transactionId = 123;
        var userId = 1;
        var user = new User { Id = userId, Balance = 100 };
        var transaction = new Transaction { Id = transactionId, UserId = userId, Amount = 50,IsIncome = true};
    
        userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        appUnitOfWorkMock.Setup(m => m.TransactionRepo.FindAsync(transactionId)).ReturnsAsync(transaction);
        userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        await walletServices.ApproveTransaction(transactionId);

        // Assert
        Assert.True(transaction.IsSuccess);
        userManagerMock.Verify(m => m.UpdateAsync(user), Times.Once);
        Assert.Equal(150, user.Balance);
    }
     [Fact]
        public async Task ApproveTransaction_WhenTransactionIsAlreadyApplied_ShouldThrowInternalServerException()
        {
            // Arrange
            var transactionId = 1;
            var transaction = new Transaction { Id = transactionId, IsSuccess = true };
            appUnitOfWorkMock.Setup(uow => uow.TransactionRepo.FindAsync(transactionId)).ReturnsAsync(transaction);

            // Act and Assert
            await Assert.ThrowsAsync<InternalServerException>(() => walletServices.ApproveTransaction(transactionId));
            appUnitOfWorkMock.Verify(uow => uow.TransactionRepo.Update(transaction), Times.Never);
            appUnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ApproveTransaction_WhenTransactionIsValid_ShouldUpdateTransactionAndSaveChanges()
        {
            // Arrange
            var transactionId = 1;
            var transaction = new Transaction { Id = transactionId, IsSuccess = false };
            appUnitOfWorkMock.Setup(uow => uow.TransactionRepo.FindAsync(transactionId)).ReturnsAsync(transaction);
            userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
            userManagerMock.Setup(um => um.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

            // Act
            await walletServices.ApproveTransaction(transactionId);

            // Assert
            appUnitOfWorkMock.Verify(uow => uow.TransactionRepo.Update(transaction), Times.Once);
            appUnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
            userManagerMock.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Once);
            userManagerMock.Verify(um => um.UpdateAsync(It.IsAny<User>()), Times.Once);
            Assert.True(transaction.IsSuccess);
        }

    
    
    [Fact]
    public async Task GetBalance_ReturnsUserBalance()
    {
        // Arrange
        var phoneNumber = "123456789";
        var user = new User { Balance = 100 };
        userManagerMock.Setup(m => m.FindByNameAsync(phoneNumber))
            .ReturnsAsync(user);

        // Act
        var result = await walletServices.GetBalance(phoneNumber);

        // Assert
        Assert.Equal(100, result);
    }

    [Fact]
    public async Task GetBalance_ThrowsNotFoundException_WhenUserNotFound()
    {
        // Arrange
        var phoneNumber = "123456789";
        userManagerMock.Setup(m => m.FindByNameAsync(phoneNumber))
            .ReturnsAsync((User)null);

        // Act and Assert
        await Assert.ThrowsAsync<NotFoundException>(() => walletServices.GetBalance(phoneNumber));
    }
    
    [Fact]
    public async Task Withdraw_WhenBalanceIsSufficient_ShouldAddTransactionAndReduceBalance()
    {
        // Arrange
        var userId = 1;
        var amount = 100;
        var cardNo = "1234567890";
        var user = new User { Id = userId, Balance = 200 };
        var transactionRepoMock = new Mock<IGenericRepo<Transaction>>();
        userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
        appUnitOfWorkMock.Setup(uow => uow.TransactionRepo).Returns(transactionRepoMock.Object);
        transactionRepoMock.Setup(repo => repo.AddAsync(It.IsAny<Transaction>())).Returns(Task.CompletedTask);

        // Act
        await walletServices.Withdraw(userId, amount, cardNo);

        // Assert
        transactionRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Transaction>()), Times.Once);
        appUnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        userManagerMock.Verify(m => m.UpdateAsync(user), Times.Exactly(2));
        Assert.Equal(100, user.Balance);
    }

    [Fact]
    public async Task Withdraw_WhenBalanceIsInsufficient_ShouldThrowBadArgumentException()
    {
        // Arrange
        var userId = 1;
        var amount = 100;
        var cardNo = "1234567890";
        var user = new User { Id = userId, Balance = 50 };
        userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);

        // Act and Assert
        await Assert.ThrowsAsync<BadArgumentException>(() => walletServices.Withdraw(userId, amount, cardNo));
        appUnitOfWorkMock.Verify(uow => uow.TransactionRepo.AddAsync(It.IsAny<Transaction>()), Times.Never);
        appUnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        userManagerMock.Verify(m => m.UpdateAsync(user), Times.Never);
    }

        [Fact]
        public async Task PayWithdraw_WhenTransactionIsNotWithdraw_ShouldThrowBadArgumentException()
        {
            // Arrange
            var transactionId = 1;
            var transaction = new Transaction { Id = transactionId, IsWithdraw = false };
            appUnitOfWorkMock.Setup(uow => uow.TransactionRepo.FindAsync(transactionId)).ReturnsAsync(transaction);

            // Act and Assert
            await Assert.ThrowsAsync<BadArgumentException>(() => walletServices.PayWithdraw(transactionId));
            appUnitOfWorkMock.Verify(uow => uow.TransactionRepo.Update(transaction), Times.Never);
            appUnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task PayWithdraw_WhenTransactionIsAlreadyPaid_ShouldThrowBadArgumentException()
        {
            // Arrange
            var transactionId = 1;
            var transaction = new Transaction { Id = transactionId, IsWithdraw = true, IsPaid = true };
            appUnitOfWorkMock.Setup(uow => uow.TransactionRepo.FindAsync(transactionId)).ReturnsAsync(transaction);

            // Act and Assert
            await Assert.ThrowsAsync<BadArgumentException>(() => walletServices.PayWithdraw(transactionId));
            appUnitOfWorkMock.Verify(uow => uow.TransactionRepo.Update(transaction), Times.Never);
            appUnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task PayWithdraw_WhenTransactionIsValid_ShouldUpdateTransactionAndSaveChanges()
        {
            // Arrange
            var transactionId = 1;
            var transaction = new Transaction { Id = transactionId, IsWithdraw = true, IsPaid = false };
            appUnitOfWorkMock.Setup(uow => uow.TransactionRepo.FindAsync(transactionId)).ReturnsAsync(transaction);

            // Act
            await walletServices.PayWithdraw(transactionId);

            // Assert
            appUnitOfWorkMock.Verify(uow => uow.TransactionRepo.Update(transaction), Times.Once);
            appUnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
            Assert.True(transaction.IsPaid);
        }
        

}