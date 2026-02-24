using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using OrderService.Models;

namespace OrderService.Services;

public class ProductServiceClient
{
    private readonly HttpClient _http;

    public ProductServiceClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ProductDto>> GetProductsAsync()
    {
        // Trae la lista de ProductService y la convierte a ProductDto local
        var productsFromService = await _http.GetFromJsonAsync<List<ProductDto>>("/api/Product");
        if (productsFromService == null) return new List<ProductDto>();

        // Inicializa Quantity en 0 para el pedido
        return productsFromService.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Quantity = 0
        }).ToList();
    }
    public async Task<ProductDto?> GetProductById(int productId)
    {
        return await _http.GetFromJsonAsync<ProductDto>($"/api/Product/{productId}");
    }
}
