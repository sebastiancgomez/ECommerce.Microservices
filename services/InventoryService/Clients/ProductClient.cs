using System.Net.Http.Json;
using InventoryService.DTOs;

namespace InventoryService.Clients;

public class ProductClient : IProductClient
{
    private readonly HttpClient _httpClient;

    public ProductClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ExistsAsync(int productId)
    {
        var response = await _httpClient.GetAsync($"api/Product/{productId}");
        return response.IsSuccessStatusCode;
    }
}