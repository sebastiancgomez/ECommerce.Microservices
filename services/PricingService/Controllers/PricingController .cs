using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PricingService.Data;
using PricingService.DTOs;
using PricingService.Services;

namespace PricingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PricingController : ControllerBase
{
    private readonly IPricingService _pricingService;

    public PricingController(IPricingService pricingService)
    {
        _pricingService = pricingService;
    }

    [HttpPost("calculate")]
    public async Task<ActionResult<PricingResultDto>> GetPrice(PricingRequestDto request)
    {
       
        var finalPrice = await _pricingService.CalculatePriceAsync(
            request.ProductId,
            request.Quantity,
            request.BasePrice); 

        return Ok(finalPrice);
    }
}