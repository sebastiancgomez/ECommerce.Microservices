namespace InventoryService.Models;

public class InventoryItem
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }

    public int AvailableStock { get; private set; }
    public int ReservedStock { get; private set; }

    public int TotalStock => AvailableStock + ReservedStock;

    protected InventoryItem() { }

    public InventoryItem(int productId, int initialStock)
    {
        if (initialStock < 0)
            throw new ArgumentException("Initial stock cannot be negative");

        ProductId = productId;
        AvailableStock = initialStock;
        ReservedStock = 0;
    }

    public void Reserve(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        if (AvailableStock < quantity)
            throw new InvalidOperationException("Not enough stock available");

        AvailableStock -= quantity;
        ReservedStock += quantity;
    }

    public void Release(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        if (ReservedStock < quantity)
            throw new InvalidOperationException("Cannot release more than reserved");

        ReservedStock -= quantity;
        AvailableStock += quantity;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        AvailableStock += quantity;
    } 
}