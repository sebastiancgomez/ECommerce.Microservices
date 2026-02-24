namespace InventoryService.Models;

public class InventoryItem
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public int Stock { get; private set; }

    public InventoryItem(int productId, int stock)
    {
        ProductId = productId;
        Stock = stock;
    }

    public void Reserve(int quantity)
    {
        if (Stock < quantity)
            throw new InvalidOperationException("Not enough stock");

        Stock -= quantity;
    }

    public void Release(int quantity)
    {
        Stock += quantity;
    }

    private InventoryItem() { }
}