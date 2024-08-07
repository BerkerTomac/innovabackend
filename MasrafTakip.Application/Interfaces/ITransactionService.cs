﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MasrafTakip.Application.DTOs;
using MasrafTakip.Domain.Entities;

namespace MasrafTakip.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync(string userId);
        Task<TransactionDto> GetTransactionByIdAsync(int id, string userId);
        Task<Transaction> AddTransactionAsync(TransactionDto transaction, string userId);
        Task UpdateTransactionAsync(TransactionDto transaction, int id, string userId);
        Task DeleteTransactionAsync(int id, string userId);
        Task<decimal> GetTotalExpensesByUserIdAsync(string userId);
        Task<decimal> CalculateTotalExpensesAsync(DateTime startDate, DateTime endDate, string userId);
    }
}
