using OrderService.Models;

namespace OrderService.Services;

public class InventoryServiceClient
{
    private readonly HttpClient _http;

    public InventoryServiceClient(HttpClient http)
    {
        _http = http;
    }

   
    public async Task<bool> IsAvailable(int productId, int quantity)
    {
        return await _http.GetFromJsonAsync<bool>($"/api/Inventory/{productId}/{quantity}");
    }

    public async Task<bool> Reserve(int productId, int quantity)
    {
        return await _http.GetFromJsonAsync<bool>($"/api/Inventory/Reserve/{productId}/{quantity}");
    }

    public async Task<bool> Release(int productId, int quantity)
    {
        return await _http.GetFromJsonAsync<bool>($"/api/Inventory/Release/{productId}/{quantity}");
    }
}