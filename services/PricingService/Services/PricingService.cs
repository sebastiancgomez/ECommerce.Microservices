using Polly.CircuitBreaker;
using Polly.Timeout;
using PricingService.Clients;
using PricingService.DTOs;
using PricingService.Models;
using PricingService.Repositories;

namespace PricingService.Services;

public class PricingService : IPricingService
{
    private readonly IPricingRuleRepository _repository;
    private readonly IProductClient _productClient;
    private readonly ILogger<PricingService> _logger;

    public PricingService(IPricingRuleRepository repository, IProductClient productClient, ILogger<PricingService> logger)
    {
        _repository = repository;
        _productClient = productClient;
        _logger = logger;
    }

    public async Task CreateRuleAsync(CreatePricingRuleDto dto)
    {
        _logger.LogInformation("Creating pricing rule for product {ProductId} with min quantity {MinQuantity}, Discount Percentage {DiscountPercentage}",
            dto.ProductId, dto.MinQuantity, dto.DiscountPercentage);
        try
        {
            var productExists = await _productClient.ExistsAsync(dto.ProductId);
            if (!productExists)
            {
                _logger.LogWarning("Product {ProductId} not found", dto.ProductId);
                throw new InvalidOperationException($"Product {dto.ProductId} does not exist.");
            }

            var rule = new PricingRule(
                dto.ProductId,
                dto.MinQuantity,
                dto.DiscountPercentage,
                dto.StartDate,
                dto.EndDate
            );

            await _repository.AddAsync(rule);
            _logger.LogInformation("Pricing Rule {RuleId} created successfully for product {ProductId}",
                    rule.Id, rule.ProductId);
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(ex, "[CIRCUIT BREAKER] Circuit is open, service unavailable. Product {ProductId}",
                dto.ProductId);

            throw; // el controlador lo captura y retorna 503
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex, "[TIMEOUT] Service timed out. Product {ProductId}",
                dto.ProductId);

            throw;
        }
        catch
        {
            _logger.LogError("Pricing Rule creation failed for product {ProductId}",
                dto.ProductId);

            throw;
        }
    }

    public async Task<PricingResultDto> CalculatePriceAsync(int productId, int quantity, decimal basePrice)
    {
        _logger.LogInformation("Calculating price for product {ProductId} for {Quantity} itmes and base price {basePrice}",
            productId, quantity, basePrice);
        var rules = await _repository.GetRulesByProductIdAsync(productId);

        var now = DateTime.UtcNow;

        var validRules = rules
            .Where(r =>
                r.IsActive &&
                (!r.StartDate.HasValue || r.StartDate <= now) &&
                (!r.EndDate.HasValue || r.EndDate >= now))
            .ToList();
        _logger.LogInformation("Found {ValidRulesCount} valid pricing rules for product {ProductId}", validRules.Count, productId);
        var rule = validRules
            .Where(r => quantity >= r.MinQuantity)
            .OrderByDescending(r => r.MinQuantity)
            .FirstOrDefault();

        var discount = rule?.DiscountPercentage ?? 0;

        var finalPrice = basePrice - (basePrice * discount / 100);
        _logger.LogInformation("Final price for product {ProductId} with quantity {Quantity} is {FinalPrice} after applying discount of {DiscountPercentage}%",
            productId, quantity, finalPrice, discount);

        return new PricingResultDto
        {
            ProductId = productId,
            Quantity = quantity,
            BasePrice = basePrice,
            FinalPrice = finalPrice
        };
    }
}
