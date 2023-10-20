using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;
using Xunit;

namespace VakilPors.Core.Services.Tests
{
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
            appUnitOfWorkMock.Setup(m => m.TransactionRepo.AddAsync(It.IsAny<Tranaction>()));
            appUnitOfWorkMock.Setup(m => m.SaveChangesAsync());
            emailSenderMock.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),It.IsAny<bool>())).Returns(Task.CompletedTask);
            telegramServiceMock.Setup(m => m.SendToTelegram(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
            // Act
            await walletServices.AddTransaction(userId, amount, description, authority, isSuccess, isIncome);

            // Assert
            appUnitOfWorkMock.Verify(m => m.TransactionRepo.AddAsync(It.IsAny<Tranaction>()), Times.Once);
            appUnitOfWorkMock.Verify(m => m.SaveChangesAsync(), Times.Once);
            userManagerMock.Verify(m => m.UpdateAsync(user), Times.Once);
            emailSenderMock.Verify(m => m.SendEmailAsync(user.Email, user.Name, "تراکنش", It.IsAny<string>(),It.IsAny<bool>()), Times.Once);
            telegramServiceMock.Verify(m => m.SendToTelegram(It.IsAny<string>(), user.Telegram), Times.Once);
        }

        // Add more test methods to cover other scenarios and methods in the WalletServices class
    }
}