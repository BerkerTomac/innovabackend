using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasrafTakip.Application.DTOs;
using MasrafTakip.Application.Interfaces;
using MasrafTakip.Domain.Entities;
using MasrafTakip.Domain.Interfaces;

namespace MasrafTakip.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync(string userId)
        {
            var transactions = await _transactionRepository.GetAllByUserIdAsync(userId);
            return transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                UserId = t.UserId,
                Amount = t.Amount,
                Date = t.Date
            }).ToList();
        }

        public async Task<TransactionDto> GetTransactionByIdAsync(int id, string userId)
        {
            var transaction = await _transactionRepository.GetByIdAndUserIdAsync(id, userId);
            if (transaction == null)
                return null;

            return new TransactionDto
            {
                Id = transaction.Id,
                UserId = transaction.UserId,
                Amount = transaction.Amount,
                Date = transaction.Date
            };
        }

        public async Task AddTransactionAsync(TransactionDto transactionDto, string userId)
        {
            var transaction = new Transaction
            {
                UserId = userId,
                Amount = transactionDto.Amount,
                Date = transactionDto.Date
            };
            await _transactionRepository.AddAsync(transaction);
        }

        public async Task UpdateTransactionAsync(TransactionDto transactionDto, string userId)
        {
            var transaction = await _transactionRepository.GetByIdAndUserIdAsync(transactionDto.Id, userId);
            if (transaction != null)
            {
                transaction.UserId = userId;
                transaction.Amount = transactionDto.Amount;
                transaction.Date = transactionDto.Date;
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
