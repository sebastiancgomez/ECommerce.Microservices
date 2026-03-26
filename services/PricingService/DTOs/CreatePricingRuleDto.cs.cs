namespace PricingService.DTOs;

public class CreatePricingRuleDto
{
    public int ProductId { get; set; }

    public int MinQuantity { get; set; }

    public decimal DiscountPercentage { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}