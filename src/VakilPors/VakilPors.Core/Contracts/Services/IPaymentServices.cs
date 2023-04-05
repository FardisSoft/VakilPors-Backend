using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Shared.Services;
using ZarinSharp.OutputTypes;

namespace VakilPors.Core.Contracts.Services
{
    public interface IPaymentServices : IScopedDependency
    {
        public Task<RequestPaymentOutput> RequestPayment(int userId, string phoneNumber,long amount, string description, string callbackUrl);
        public Task<VerifyPaymentOutput> VerifyPayment(string authority, string status);
    }
}