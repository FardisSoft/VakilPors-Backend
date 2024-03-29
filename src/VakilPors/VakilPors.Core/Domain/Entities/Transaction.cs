using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using VakilPors.Shared.Entities;

namespace VakilPors.Core.Domain.Entities
{
    public class Transaction : IEntity
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string? Authority { get; set; }
        public string? Description { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsIncome { get; set; }
        public bool IsWithdraw { get; set; } = false;
        public bool IsPaid { get; set; } = false;

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}