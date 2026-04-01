namespace PaymentService.Models;

public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = CurrencyType.USD.ToString();
    public string Status { get; set; } = PaymentStatus.Pending.ToString(); // Pending, Completed, Failed
    public string Method { get; set; } = "FAKE";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
public enum CurrencyType
{
    USD,
    COP,
    EUR
}
public enum PaymentStatus
{
    Pending,
    Completed,
    Failed
}