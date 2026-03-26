namespace OrderService.DTOs
{
    public class PricingResponseDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public string DiscountType { get; set; } = "None";
        public decimal FinalPrice { get; set; }
    }
}
