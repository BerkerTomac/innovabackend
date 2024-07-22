using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasrafTakip.Domain.Entities;
using MasrafTakip.Domain.Interfaces;
using MasrafTakip.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MasrafTakip.Infrastructure.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Transaction>> GetAllByUserIdAsync(string userId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<Transaction> GetByIdAndUserIdAsync(int id, string userId)
        {
            return await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task<IEnumerable<Transaction>> GetByDateRangeAndUserIdAsync(DateTime startDate, DateTime endDate, string userId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate)
                .ToListAsync();
        }

        public override async Task DeleteAsync(Transaction entity)
        {
            _context.Set<Transaction>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
