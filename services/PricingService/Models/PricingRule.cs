namespace PricingService.Models;

public class PricingRule
{
    public int Id { get; private set; }

    public int ProductId { get; private set; }

    public int MinQuantity { get; private set; }

    public decimal DiscountPercentage { get; private set; }

    public DateTime? StartDate { get; private set; }

    public DateTime? EndDate { get; private set; }

    public bool IsActive { get; private set; }

    public PricingRule(
        int productId,
        int minQuantity,
        decimal discountPercentage,
        DateTime? startDate,
        DateTime? endDate)
    {
        ProductId = productId;
        MinQuantity = minQuantity;
        DiscountPercentage = discountPercentage;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = true;
    }

    private PricingRule() { }
}