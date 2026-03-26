namespace OrderService.DTOs
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Status { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }
}
