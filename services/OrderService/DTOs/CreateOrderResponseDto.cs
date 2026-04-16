namespace OrderService.DTOs;

public class CreateOrderResponseDto
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public string PaymentUrl { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
}