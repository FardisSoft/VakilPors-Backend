using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Exceptions;
using ZarinSharp;
using ZarinSharp.OutputTypes;

namespace VakilPors.Core.Services
{
    public class PaymentServices : IPaymentServices
    {
        private readonly ZarinPalService _zarinpal;
        private readonly IWalletServices walletServices;
        private readonly IAppUnitOfWork appUnitOfWork;

        public PaymentServices(ZarinPalService zarinPalService, IWalletServices walletServices,IAppUnitOfWork appUnitOfWork)
        {
            this._zarinpal = zarinPalService;
            this.walletServices = walletServices;
            this.appUnitOfWork = appUnitOfWork;
        }
        public async Task<RequestPaymentOutput> RequestPayment(int userId,long amount, string description, string callbackUrl)
        {
            var requestResult = await _zarinpal.RequestPaymentAsync(new()
            {
                Amount = amount,
                CallbackUrl = new Uri(callbackUrl),
                Description = description
            });
            if (!requestResult.WasSuccessful)
            {
                throw new BadArgumentException("خطا در درخواست پرداخت");
            }
            await walletServices.AddTransaction(userId,amount,description,requestResult.Authority,false,true);
            return requestResult;
        }

        public async Task<VerifyPaymentOutput> VerifyPayment(string authority, string status)
        {
            var tranaction=appUnitOfWork.TransactionRepo.AsQueryableNoTracking().FirstOrDefault(t=>t.Authority==authority);
            var amount=tranaction.Amount;
            var verificationResult = await _zarinpal.VerifyPaymentAsync(new()
            {
                Amount = Convert.ToInt64(amount),
                Authority = authority,
            });
            if (!verificationResult.WasSuccessful)
            {
                throw new BadArgumentException("خطا در تایید پرداخت");
            }
            await walletServices.ApproveTransaction(tranaction.Id);
            return verificationResult;
        }
    }
}