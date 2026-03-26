using OrderService.DTOs;
using System.Net.Http.Json;

namespace OrderService.Clients;

public class PricingClient : IPricingClient
{

    private readonly HttpClient _httpClient;

    public PricingClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PricingResponseDto> GetPrice(PricingRequestDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync<PricingRequestDto>(
            $"api/Pricing/Calculate", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PricingResponseDto>()!;

    }
}