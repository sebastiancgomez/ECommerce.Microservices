using OrderService.DTOs;
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
        var inventory = await _httpClient.GetFromJsonAsync<InventoryDto>($"api/Inventory/{productId}");
        Console.WriteLine($"Inventory for product {productId}: {inventory?.Stock}");
        if (inventory is null)
            return false;
        if(inventory.Stock < quantity)
            return false;
        return true;
    }

    public async Task Reserve(int productId, int quantity)
    {
        ReserveStockDto dto = new ReserveStockDto
        {
            ProductId = productId,
            Quantity = quantity
        };
        await _httpClient.PostAsJsonAsync<ReserveStockDto>(
            $"api/Inventory/Reserve", dto);
       
    }
}