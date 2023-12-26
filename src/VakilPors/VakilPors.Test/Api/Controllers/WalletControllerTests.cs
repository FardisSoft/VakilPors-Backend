using Pagination.EntityFrameworkCore.Extensions;
using VakilPors.Api.Controllers;

namespace VakilPors.Test.Api.Controllers;

using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.Payment;
using VakilPors.Core.Domain.Dtos.Transaction;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Response;
using Xunit;

public class WalletControllerTests
{
    private readonly Mock<IWalletServices> _walletServicesMock = new Mock<IWalletServices>();
    private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
    private readonly Mock<ILogger<WalletController>> _loggerMock = new Mock<ILogger<WalletController>>();
    private readonly WalletController _controller;

    public WalletControllerTests()
    {
        // Initialize controller with mocked services and setup simulated user context
        _controller = new WalletController(_walletServicesMock.Object, _mapperMock.Object, _loggerMock.Object);

        // Assuming these methods are available to mock user information
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext // Mock HttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim("uid", "1"),
                    new Claim(ClaimTypes.NameIdentifier, "phoneNumber")
                }, "TestAuthType"))
            }
        };
    }

    [Fact]
    public async Task GetBalance_CallsGetBalanceAndLogsInformation()
    {
        // Arrange
        decimal expectedBalance = 100.00M;
        _walletServicesMock.Setup(x => x.GetBalance(It.IsAny<string>()))
            .ReturnsAsync(expectedBalance);

        // Act
        var result = await _controller.GetBalance();

        // Assert
        Assert.Equal(expectedBalance, result);
    }

    // ... subsequent tests for AddBalance, MakeWithdraw, PayWithdraw, etc.

    [Fact]
    public async Task MakeWithdraw_ReturnsOkWhenSuccessful()
    {
        // Arrange
        decimal amount = 100.00M;
        string cardNo = "1234567890123456";

        _walletServicesMock.Setup(x => x.Withdraw(It.IsAny<int>(), amount, cardNo))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.MakeWithdraw(amount, cardNo);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var appResponse = Assert.IsType<AppResponse>(okResult.Value);
        Assert.Equal(HttpStatusCode.OK, appResponse.StatusCode);
    }

    // ... additional test methods for PayWithdraw and GetWithdrawTransactions

    [Fact]
    public async Task GetWithdrawTransactions_ReturnsOkWithTransactionList()
    {
        // Arrange
        var transactions = new List<Transaction> { /* fill with test data */ };
        var transactionDtos = new List<TransactionDto> { /* fill with mapped DTOs */ };

        _walletServicesMock.Setup(x => x.GetWithdrawTransactions())
            .ReturnsAsync(transactions);

        _mapperMock.Setup(m => m.Map<IEnumerable<Transaction>, IEnumerable<TransactionDto>>(It.IsAny<IEnumerable<Transaction>>()))
            .Returns(transactionDtos);

        // Act
        var result = await _controller.GetWithdrawTransactions();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var appResponse = Assert.IsType<AppResponse<IEnumerable<TransactionDto>>>(okResult.Value);
        Assert.Equal(transactionDtos, appResponse.Data);
    }

// Continuing the WalletControllerTests class...

[Fact]
public async Task AddBalance_CallsServiceAndLogsInformation()
{
    // Arrange
    decimal amountToAdd = 50.00M;
    string phoneNumber = "1234567890";
    _walletServicesMock.Setup(x => x.AddBalance(phoneNumber, amountToAdd))
        .Returns(Task.CompletedTask);

    // Note: If any AddBalance asserts or preconditions are needed, you should set them up before calling the method.

    // Act
    await _controller.AddBalance(amountToAdd, phoneNumber);

    // Assert
    _walletServicesMock.Verify(service => service.AddBalance(phoneNumber, amountToAdd), Times.Once);
}

[Fact]
public async Task GetTransactions_ReturnsPaginatedTransactionDtos()
{
    // Arrange
    var pagedParams = new PagedParams { PageNumber = 1, PageSize = 10 };
    var sortParams = new SortParams { Sort = "Date", IsAscending = false };

    var transactionEntities = new Pagination<Transaction>(new List<Transaction>(),10)
    {
        // ... Populate with test data
    };

    var transactionDtos = new Pagination<TransactionDto>(new List<TransactionDto>(),10)
    {
        // ... Populate with potentially mapped data 
    };

    _walletServicesMock.Setup(svc => svc.GetTransactions(It.IsAny<string>(), pagedParams, sortParams))
        .ReturnsAsync(transactionEntities);

    _mapperMock.Setup(mapper => mapper.Map<Pagination<TransactionDto>>(transactionEntities))
        .Returns(transactionDtos);

    // Act
    var result = await _controller.GetTransactions(pagedParams, sortParams);

    // Assert
    var pagedResult = Assert.IsType<Pagination<TransactionDto>>(result);
}

[Fact]
public async Task PayWithdraw_CallsServiceAndLogsInformation()
{
    // Arrange
    int transactionId = 123;
    _walletServicesMock.Setup(x => x.PayWithdraw(transactionId))
        .Returns(Task.CompletedTask);

    // Act
    var result = await _controller.PayWithdraw(transactionId);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var appResponse = Assert.IsType<AppResponse>(okResult.Value);
    Assert.Equal(HttpStatusCode.OK, appResponse.StatusCode);
    _walletServicesMock.Verify(service => service.PayWithdraw(transactionId), Times.Once);
}
[Fact]
public async Task GetMonthlyTransactionsAmount_ReturnsCorrectData()
{
    // Arrange
    var monthlyTransactionAmount = new List<MonthlyTransactionAmountDto>
    {
        new MonthlyTransactionAmountDto { Month = "Farvardin", Amount = 100 },
        new MonthlyTransactionAmountDto { Month = "Ordibehesht", Amount = 200 }
        // ... other months as necessary
    };

    var asyncMonthlyTransactionAmount = GetTestValues();

    _walletServicesMock.Setup(w => w.GetMonthlyTransactionsAmount(It.IsAny<int>()))
        .Returns(asyncMonthlyTransactionAmount);

    // Act
    var result = await _controller.GetMonthlyTransactionsAmount();

    // Assert
    var okObjectResult = Assert.IsType<OkObjectResult>(result);
    var appResponse = Assert.IsAssignableFrom<AppResponse<IAsyncEnumerable<MonthlyTransactionAmountDto>>>(okObjectResult.Value);
    Assert.NotNull(appResponse.Data);
        
    var resultList = new List<MonthlyTransactionAmountDto>();
    await foreach (var item in appResponse.Data)
    {
        resultList.Add(item);
    }
        
    Assert.Equal(monthlyTransactionAmount.Count, resultList.Count);
    for (int i = 0; i < monthlyTransactionAmount.Count; i++)
    {
        Assert.Equal(monthlyTransactionAmount[i].Month, resultList[i].Month);
        Assert.Equal(monthlyTransactionAmount[i].Amount, resultList[i].Amount);
    }
    async IAsyncEnumerable<MonthlyTransactionAmountDto> GetTestValues()
    {
        foreach (var testData in monthlyTransactionAmount)
        {
            yield return testData;
        }

        await Task.CompletedTask; // to make the compiler warning go away
    }

}
}
