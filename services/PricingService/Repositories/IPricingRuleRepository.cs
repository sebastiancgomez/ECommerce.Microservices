namespace PricingService.Repositories;

using PricingService.Models;

public interface IPricingRuleRepository
{
    Task AddAsync(PricingRule rule);

    Task<List<PricingRule>> GetRulesByProductIdAsync(int productId);
}