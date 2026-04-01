namespace OrderService.DTOs;

public class PaymentResponseDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string Method { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
}
