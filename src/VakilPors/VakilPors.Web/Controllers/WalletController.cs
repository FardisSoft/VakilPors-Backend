using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;
using X.PagedList;

namespace VakilPors.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class WalletController : MyControllerBase
    {
        private readonly IWalletServices walletServices;

        public WalletController(IWalletServices walletServices)
        {
            this.walletServices = walletServices;
        }
        [HttpGet("GetBalance")]
        public async Task<decimal> GetBalance()
        {
            var phoneNumber= getPhoneNumber();
            return await walletServices.GetBalance(phoneNumber);
        }
        [Authorize(Roles=RoleNames.Admin)]
        [HttpPost("AddBalance")]
        public async Task AddBalance(decimal amount,string phoneNumber)
        {
            await walletServices.AddBalance(phoneNumber, amount);
        }
        //get transactions
        [HttpGet("GetTransactions")]
        public async Task<IPagedList<Tranaction>> GetTransactions([FromQuery]PagedParams pagedParams)
        {
            var phoneNumber= getPhoneNumber();
            var transactions=await walletServices.GetTransactions(phoneNumber,pagedParams);
            return transactions;
        }
    }
}