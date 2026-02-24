namespace PricingService.Models;

public class PricingRule
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public decimal BasePrice { get; private set; }

    public PricingRule(int productId, decimal basePrice)
    {
        ProductId = productId;
        BasePrice = basePrice;
    }

    private PricingRule() { }
}