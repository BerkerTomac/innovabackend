using MasrafTakip.Infrastructure.Identity;
using System;

namespace MasrafTakip.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public ApplicationUser User { get; set; }
    }
}
