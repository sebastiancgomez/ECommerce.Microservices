namespace PricingService.Clients
{
    public interface IProductClient
    {
        Task<bool> ExistsAsync(int productId);
    }
}
