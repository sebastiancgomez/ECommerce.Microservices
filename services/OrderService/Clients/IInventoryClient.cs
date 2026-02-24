namespace OrderService.Clients
{
    public interface IInventoryClient
    {
        Task<bool> IsAvailable(int productId, int quantity);
        Task Reserve(int productId, int quantity);
    }
}
