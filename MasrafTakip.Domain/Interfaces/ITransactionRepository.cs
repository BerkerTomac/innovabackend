using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MasrafTakip.Domain.Entities;

namespace MasrafTakip.Domain.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetAllByUserIdAsync(string userId);
        Task<Transaction> GetByIdAndUserIdAsync(int id, string userId);
        Task<IEnumerable<Transaction>> GetByDateRangeAndUserIdAsync(DateTime startDate, DateTime endDate, string userId);
    }
}
