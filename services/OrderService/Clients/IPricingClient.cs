using OrderService.DTOs;

namespace OrderService.Clients
{
    public interface IPricingClient
    {
        Task<PricingResponseDto> GetPrice(PricingRequestDto dto);
    }
}
