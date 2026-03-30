namespace OrderService.Events;

public class OrderCreatedEvent
{
    public int OrderId { get; init; }
    public int CustomerId { get; init; }
    public decimal Total { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public List<OrderCreatedEventItem> Items { get; init; } = [];
}

public class OrderCreatedEventItem
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}