using Hangfire;
using MasrafTakip.Application.Interfaces;
using MasrafTakip.Domain.Entities;
using MasrafTakip.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace MasrafTakip.Infrastructure.BackgroundJobs
{
    public class TransactionAggregationJob
    {
        private readonly ITransactionService _transactionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TransactionAggregationJob(ITransactionService transactionService, UserManager<ApplicationUser> userManager)
        {
            _transactionService = transactionService;
            _userManager = userManager;
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task AggregateDailyTransactions()
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-1);
            var endDate = DateTime.UtcNow.Date;

            var users = _userManager.Users;
            foreach (var user in users)
            {
                await _transactionService.CalculateTotalExpensesAsync(startDate, endDate, user.Id);
            }
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task AggregateWeeklyTransactions()
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-7);
            var endDate = DateTime.UtcNow.Date;

            var users = _userManager.Users;
            foreach (var user in users)
            {
                await _transactionService.CalculateTotalExpensesAsync(startDate, endDate, user.Id);
            }
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task AggregateMonthlyTransactions()
        {
            var startDate = DateTime.UtcNow.Date.AddMonths(-1);
            var endDate = DateTime.UtcNow.Date;

            var users = _userManager.Users;
            foreach (var user in users)
            {
                await _transactionService.CalculateTotalExpensesAsync(startDate, endDate, user.Id);
            }
        }
    }
}
