using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.Payment;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;
using VakilPors.Core.Mapper;
using X.PagedList;

namespace VakilPors.Web.Controllers
{

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
            var phoneNumber = getPhoneNumber();
            _logger.LogInformation($"get balance for user with phone number:{phoneNumber}");
            return await _walletServices.GetBalance(phoneNumber);
        }
        [Authorize(Roles = RoleNames.Admin)]
        [HttpPost("AddBalance")]
        public async Task AddBalance(decimal amount, string phoneNumber)
        {
            _logger.LogInformation($"add balance with amount:{amount} for user with phone number:{phoneNumber} by admin with phone number:{getPhoneNumber()}");
            await _walletServices.AddBalance(phoneNumber, amount);
        }
        //get transactions
        [HttpGet("GetTransactions")]
        public async Task<IPagedList<TranactionDto>> GetTransactions([FromQuery] PagedParams pagedParams)
        {
            var phoneNumber = getPhoneNumber();
            _logger.LogInformation($"get transactions for user with phone number:{phoneNumber}");
            IPagedList<Tranaction> transactions = await _walletServices.GetTransactions(phoneNumber, pagedParams);
            return transactions.ToMappedPagedList<Tranaction, TranactionDto>(_mapper);
        }
    }
}