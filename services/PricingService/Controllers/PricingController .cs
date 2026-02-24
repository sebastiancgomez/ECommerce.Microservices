using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PricingService.Data;

namespace PricingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PricingController : ControllerBase
{
    private readonly PricingDbContext _db;

    public PricingController(PricingDbContext db)
    {
        _db = db;
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<decimal>> GetPrice(int productId)
    {
        var rule = await _db.PricingRules
            .FirstOrDefaultAsync(r => r.ProductId == productId);

        if (rule == null)
            return NotFound();

        return rule.BasePrice;
    }
}