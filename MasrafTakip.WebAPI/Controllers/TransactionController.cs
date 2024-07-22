using MasrafTakip.Application.DTOs;
using MasrafTakip.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ITransactionService transactionService, ILogger<TransactionController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TransactionDto transactionDto)
        {
            if (transactionDto == null)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue("UserId"); 
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            _logger.LogInformation("User ID from claims: {UserId}", userId);

            var createdTransaction = await _transactionService.AddTransactionAsync(transactionDto, userId);
            return CreatedAtAction(nameof(GetById), new { id = createdTransaction.Id }, createdTransaction);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirstValue("UserId"); 
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            _logger.LogInformation("User ID from claims: {UserId}", userId);

            var transaction = await _transactionService.GetTransactionByIdAsync(id, userId);
            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TransactionDto transactionDto)
        {
            if (transactionDto == null)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue("UserId"); 
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            _logger.LogInformation("User ID from claims: {UserId}", userId);

            await _transactionService.UpdateTransactionAsync(transactionDto, id, userId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue("UserId"); 
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            _logger.LogInformation("User ID from claims: {UserId}", userId);

            await _transactionService.DeleteTransactionAsync(id, userId);
            return NoContent();
        }
    }
}
