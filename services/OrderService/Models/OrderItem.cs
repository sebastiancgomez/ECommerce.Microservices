namespace OrderService.Models;

public class OrderItem
{
    public int Id { get; set; }

    // Producto relacionado (solo datos necesarios, desde ProductService)
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }

    public int Quantity { get; set; }
}