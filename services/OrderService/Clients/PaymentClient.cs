using OrderService.DTOs;

namespace OrderService.Clients;

public class PaymentClient : IPaymentClient
{
    private readonly HttpClient _httpClient;

    public PaymentClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PaymentResponseDto> CreatePayment(CreatePaymentRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/payment", request);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PaymentResponseDto>();
    }
}
