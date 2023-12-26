using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination.EntityFrameworkCore.Extensions;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.Payment;
using VakilPors.Core.Domain.Dtos.Transaction;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Mapper;
using VakilPors.Shared.Response;

namespace VakilPors.Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class WalletController : MyControllerBase
{
    private readonly IWalletServices _walletServices;
    private readonly IMapper _mapper;
    private readonly ILogger<WalletController> _logger;

    public WalletController(IWalletServices walletServices, IMapper mapper, ILogger<WalletController> logger)
    {
        this._logger = logger;
        this._mapper = mapper;
        this._walletServices = walletServices;
    }
    [HttpGet("GetBalance")]
    public async Task<decimal> GetBalance()
    {
        var phoneNumber = GetPhoneNumber();
        _logger.LogInformation($"get balance for user with phone number:{phoneNumber}");
        return await _walletServices.GetBalance(phoneNumber);
    }
    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost("AddBalance")]
    public async Task AddBalance(decimal amount, string phoneNumber)
    {
        _logger.LogInformation($"add balance with amount:{amount} for user with phone number:{phoneNumber} by admin with phone number:{GetPhoneNumber()}");
        await _walletServices.AddBalance(phoneNumber, amount);
    }
    //get transactions
    [HttpGet("GetTransactions")]
    public async Task<Pagination<TransactionDto>> GetTransactions([FromQuery] PagedParams pagedParams,[FromQuery] SortParams sortParams)
    {
        var phoneNumber = GetPhoneNumber();
        _logger.LogInformation($"get transactions for user with phone number:{phoneNumber}");
        Pagination<Transaction> transactions = await _walletServices.GetTransactions(phoneNumber, pagedParams,sortParams);
        return transactions.ToMappedPagination<Transaction, TransactionDto>(_mapper, pagedParams.PageSize);
    }

    [Authorize(Roles = RoleNames.Vakil)]
    [HttpPost("MakeWithdraw")]
    public async Task<ActionResult> MakeWithdraw(decimal amount, string cardNo)
    {
        var userId = GetUserId();
        var phoneNumber = GetPhoneNumber();
        _logger.LogInformation($"withdraw balance with amount:{amount} for lawyer with phone number:{phoneNumber}");
        await _walletServices.Withdraw(userId, amount, cardNo);
        return Ok(new AppResponse(HttpStatusCode.OK, $"Withdrawn was successful!"));
    }
    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost("PayWithdraw")]
    public async Task<IActionResult> PayWithdraw(int transactionId)
    {
        var phoneNumber = GetPhoneNumber();
        _logger.LogInformation($"admin with phone number:{phoneNumber} paid transaction with id:{transactionId}");
        await _walletServices.PayWithdraw(transactionId);
        return Ok(new AppResponse(HttpStatusCode.OK, $"Withdraw has been paid successfully!"));
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet("GetWithdrawTransactions")]
    public async Task<IActionResult> GetWithdrawTransactions()
    {
        var phoneNumber = GetPhoneNumber();
        _logger.LogInformation($"get transactions for user with phone number:{phoneNumber}");
        IEnumerable<Transaction> transactions = await _walletServices.GetWithdrawTransactions();
        IEnumerable<TransactionDto> result = _mapper.Map<IEnumerable<Transaction>, IEnumerable<TransactionDto>>(transactions);
        return Ok(new AppResponse<IEnumerable<TransactionDto>>(result, "Withdrawn Transactions fetched sueccessfully!"));
    }
    [HttpGet("GetMonthlyTransactionsAmount")]
    public Task<IActionResult> GetMonthlyTransactionsAmount()
    {
        var phoneNumber = GetPhoneNumber();
        var userId = GetUserId();
        _logger.LogInformation($"get monthly transactions amount for user with phone number:{phoneNumber}");
        IAsyncEnumerable<MonthlyTransactionAmountDto> result = _walletServices.GetMonthlyTransactionsAmount(userId);
        return Task.FromResult<IActionResult>(Ok(new AppResponse<IAsyncEnumerable<MonthlyTransactionAmountDto>>(result, "Monthly Transactions amount fetched successfully!")));
    }
}