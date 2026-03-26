namespace PricingService.DTOs;

public class PricingResultDto
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal BasePrice { get; set; }

    public decimal FinalPrice { get; set; }
}