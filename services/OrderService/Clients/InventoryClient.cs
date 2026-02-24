using System.Net.Http.Json;

namespace OrderService.Clients;

public class InventoryClient: IInventoryClient
{
    private readonly HttpClient _httpClient;

    public InventoryClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> IsAvailable(int productId, int quantity)
    {
        return await _httpClient.GetFromJsonAsync<bool>(
            $"api/Inventory/{productId}/{quantity}");
    }

    public async Task Reserve(int productId, int quantity)
    {
        await _httpClient.GetAsync(
            $"api/Inventory/Reserve/{productId}/{quantity}");
    }
}