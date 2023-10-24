using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VakilPors.Core.Domain.Dtos.Payment
{
    public record TransactionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsIncome { get; set; }
        public bool IsWithdraw { get; set; }
        public bool IsPaid { get; set; }
    }
}