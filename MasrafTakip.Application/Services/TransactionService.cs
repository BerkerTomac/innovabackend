using MasrafTakip.Application.DTOs;
using MasrafTakip.Application.Interfaces;
using MasrafTakip.Domain.Entities;
using MasrafTakip.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasrafTakip.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(ITransactionRepository transactionRepository, ILogger<TransactionService> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync(string userId)
        {
            var transactions = await _transactionRepository.GetAllByUserIdAsync(userId);
            return transactions.Select(t => new TransactionDto
            {
                Amount = t.Amount,
                Date = t.Date,
                Description = t.Description
            }).ToList();
        }

        public async Task<TransactionDto> GetTransactionByIdAsync(int id, string userId)
        {
            var transaction = await _transactionRepository.GetByIdAndUserIdAsync(id, userId);
            if (transaction == null)
                return null;

            return new TransactionDto
            {
                Amount = transaction.Amount,
                Date = transaction.Date,
                Description = transaction.Description
            };
        }

        public async Task<Transaction> AddTransactionAsync(TransactionDto transactionDto, string userId)
        {
            var transaction = new Transaction
            {
                UserId = userId,
                Amount = transactionDto.Amount,
                Date = transactionDto.Date,
                Description = transactionDto.Description
            };
            await _transactionRepository.AddAsync(transaction);
            return transaction;
        }

        public async Task UpdateTransactionAsync(TransactionDto transactionDto, int id, string userId)
        {
            var transaction = await _transactionRepository.GetByIdAndUserIdAsync(id, userId);
            if (transaction != null)
            {
                transaction.Amount = transactionDto.Amount;
                transaction.Date = transactionDto.Date;
                transaction.Description = transactionDto.Description;
                await _transactionRepository.UpdateAsync(transaction);
            }
        }

        public async Task DeleteTransactionAsync(int id, string userId)
        {
            var transaction = await _transactionRepository.GetByIdAndUserIdAsync(id, userId);
            if (transaction != null)
            {
                await _transactionRepository.DeleteAsync(transaction);
            }
        }

        public async Task<decimal> GetTotalExpensesByUserIdAsync(string userId)
        {
            var transactions = await _transactionRepository.GetAllByUserIdAsync(userId);
            return transactions.Sum(t => t.Amount);
        }

        public async Task<decimal> CalculateTotalExpensesAsync(DateTime startDate, DateTime endDate, string userId)
        {
            var transactions = await _transactionRepository.GetByDateRangeAndUserIdAsync(startDate, endDate, userId);
            return transactions.Sum(t => t.Amount);
        }
    }
}
