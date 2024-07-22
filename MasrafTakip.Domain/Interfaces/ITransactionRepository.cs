using MasrafTakip.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasrafTakip.Domain.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetAllByUserIdAsync(string userId);
        Task<Transaction> GetByIdAndUserIdAsync(int id, string userId);
        Task<IEnumerable<Transaction>> GetByDateRangeAndUserIdAsync(DateTime startDate, DateTime endDate, string userId);
    }
}
