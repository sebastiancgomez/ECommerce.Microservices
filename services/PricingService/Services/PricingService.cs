using PricingService.DTOs;
using PricingService.Models;
using PricingService.Repositories;

namespace PricingService.Services;

public class PricingService : IPricingService
{
    private readonly IPricingRuleRepository _repository;

    public PricingService(IPricingRuleRepository repository)
    {
        _repository = repository;
    }

    public async Task CreateRuleAsync(CreatePricingRuleDto dto)
    {
        var rule = new PricingRule(
            dto.ProductId,
            dto.MinQuantity,
            dto.DiscountPercentage,
            dto.StartDate,
            dto.EndDate
        );

        await _repository.AddAsync(rule);
    }

    public async Task<PricingResultDto> CalculatePriceAsync(int productId, int quantity, decimal basePrice)
    {
        var rules = await _repository.GetRulesByProductIdAsync(productId);

        var now = DateTime.UtcNow;

        var validRules = rules
            .Where(r =>
                r.IsActive &&
                (!r.StartDate.HasValue || r.StartDate <= now) &&
                (!r.EndDate.HasValue || r.EndDate >= now))
            .ToList();

        var rule = validRules
            .Where(r => quantity >= r.MinQuantity)
            .OrderByDescending(r => r.MinQuantity)
            .FirstOrDefault();

        var discount = rule?.DiscountPercentage ?? 0;

        var finalPrice = basePrice - (basePrice * discount / 100);

        return new PricingResultDto
        {
            ProductId = productId,
            Quantity = quantity,
            BasePrice = basePrice,
            FinalPrice = finalPrice
        };
    }
}
