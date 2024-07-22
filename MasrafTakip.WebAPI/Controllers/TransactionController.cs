using MasrafTakip.Application.DTOs;
using MasrafTakip.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MasrafTakip.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transactions = await _transactionService.GetAllTransactionsAsync(userId);
            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transaction = await _transactionService.GetTransactionByIdAsync(id, userId);
            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TransactionDto transaction)
        {
            if (transaction == null)
                return BadRequest();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _transactionService.AddTransactionAsync(transaction, userId);
            return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TransactionDto transaction)
        {
            if (transaction == null || transaction.Id != id)
                return BadRequest();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingTransaction = await _transactionService.GetTransactionByIdAsync(id, userId);
            if (existingTransaction == null)
                return NotFound();

            await _transactionService.UpdateTransactionAsync(transaction, userId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _transactionService.DeleteTransactionAsync(id, userId);
            return NoContent();
        }

        [HttpGet("total-expenses")]
        public async Task<IActionResult> GetTotalExpenses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var totalExpenses = await _transactionService.GetTotalExpensesByUserIdAsync(userId);
            return Ok(new { TotalExpenses = totalExpenses });
        }
    }
}
