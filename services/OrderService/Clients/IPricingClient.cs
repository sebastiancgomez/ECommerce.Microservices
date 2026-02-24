using OrderService.Models;

namespace OrderService.Clients
{
    public interface IPricingClient
    {
        Task<decimal> GetPrice(int productId);
    }
}
