namespace MasrafTakip.Application.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
