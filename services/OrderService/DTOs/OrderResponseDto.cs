namespace OrderService.DTOs
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public required string Status { get; set; }
        public decimal Total { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
