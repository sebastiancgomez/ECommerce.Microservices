using Microsoft.EntityFrameworkCore;
using PricingService.Data;
using PricingService.Models;

namespace PricingService.Repositories;
public class PricingRuleRepository : IPricingRuleRepository
{
    private readonly PricingDbContext _context;

    public PricingRuleRepository(PricingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PricingRule rule)
    {
        _context.PricingRules.Add(rule);
        await _context.SaveChangesAsync();
    }

    public async Task<List<PricingRule>> GetRulesByProductIdAsync(int productId)
    {
        return await _context.PricingRules
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.MinQuantity)
            .ToListAsync();
    }
}
