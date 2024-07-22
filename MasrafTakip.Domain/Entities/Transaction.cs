using System;
using MasrafTakip.Infrastructure.Identity;

namespace MasrafTakip.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public ApplicationUser User { get; set; }
    }
}
