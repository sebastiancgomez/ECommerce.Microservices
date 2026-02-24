namespace OrderService.Models;

public class OrderItem
{
    public int Id { get; private set; }

    public int OrderId { get; private set; }
    public Order Order { get; private set; } = null!; // EF Core lo inicializa

    public int ProductId { get; private set; }
    public string ProductName { get; private set; } = null!; // EF Core lo inicializa
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    public decimal TotalPrice => UnitPrice * Quantity;

    // Constructor público para crear nuevos OrderItems
    public OrderItem(int productId, string productName, decimal unitPrice, int quantity)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public void IncreaseQuantity(int quantity)
    {
        Quantity += quantity;
    }

    // Constructor vacío requerido por EF Core
    private OrderItem() { }

}
