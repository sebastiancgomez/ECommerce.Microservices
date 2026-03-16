using PricingService.DTOs;

namespace PricingService.Services;

public interface IPricingService
{
    Task CreateRuleAsync(CreatePricingRuleDto dto);

    Task<PricingResultDto> CalculatePriceAsync(int productId, int quantity, decimal basePrice);
}
