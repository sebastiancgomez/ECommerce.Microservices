namespace InventoryService.Clients
{
    public interface IProductClient
    {
        Task<bool> ExistsAsync(int productId);
    }
}
