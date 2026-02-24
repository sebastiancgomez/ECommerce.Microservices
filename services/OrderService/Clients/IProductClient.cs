using OrderService.Models;

namespace OrderService.Clients
{
    public interface IProductClient
    {
        Task<List<ProductDto>> GetProductsAsync();
        Task<ProductDto?> GetProductById(int productId);
    }
}
