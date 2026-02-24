using OrderService.Models;

namespace OrderService.Services;

public class PricingServiceClient
{
    private readonly HttpClient _http;

    public PricingServiceClient(HttpClient http)
    {
        _http = http;
    }

   
    public async Task<PricingInfo?> GetPriceAsync(int productId)
    {
        return await _http.GetFromJsonAsync<PricingInfo>($"/api/Pricing/{productId}");
    }
}