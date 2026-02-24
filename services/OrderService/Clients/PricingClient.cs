using OrderService.Models;
using System.Net.Http.Json;

namespace OrderService.Clients;

public class PricingClient : IPricingClient
{

    private readonly HttpClient _httpClient;

    public PricingClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<decimal> GetPrice(int productId)
    {
        return await _httpClient.GetFromJsonAsync<decimal>(
            $"api/Pricing/{productId}");
    }
}