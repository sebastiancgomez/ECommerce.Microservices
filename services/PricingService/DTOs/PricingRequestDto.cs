namespace PricingService.DTOs
{
    public class PricingRequestDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal BasePrice { get; set; }
    }
}
