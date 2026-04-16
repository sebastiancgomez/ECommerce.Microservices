namespace OrderService.Models;

public class Order
{
    public int Id { get; private set; }
    public int CustomerId { get; private set; }  

    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    private readonly List<OrderItem> _items = new();
    private readonly List<OrderStatusHistory> _statusHistory = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal Total => _items.Sum(i => i.TotalPrice);
    public OrderStatus Status { get; private set; } = OrderStatus.Created;
    public IReadOnlyCollection<OrderStatusHistory> StatusHistory => _statusHistory.AsReadOnly();

    public Order(int customerId) 
    {
        CustomerId = customerId;
    }
    private Order() { } 

    public void AddItem(int productId, string productName, decimal unitPrice, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.IncreaseQuantity(quantity);
            return;
        }

        var item = new OrderItem(productId, productName, unitPrice, quantity);
        _items.Add(item);
    }
    public void ChangeStatus(OrderStatus newStatus)
    {
        if (Status == newStatus) return;

        _statusHistory.Add(new OrderStatusHistory
        {
            PreviousStatus = Status,
            NewStatus = newStatus,
            ChangedAt = DateTimeOffset.UtcNow
        });

        Status = newStatus;
    }
}
public enum OrderStatus
{
    Created,
    Confirmed,
    PendingPayment,
    PaymentProcessing,
    Paid,
    Cancelled,
    Expired
}

public class OrderStatusHistory
{
    public int Id { get; set; }
    public OrderStatus PreviousStatus { get; set; }
    public OrderStatus NewStatus { get; set; }
    public DateTimeOffset ChangedAt { get; set; }
}