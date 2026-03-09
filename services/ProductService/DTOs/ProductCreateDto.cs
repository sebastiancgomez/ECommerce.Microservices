namespace ProductService.DTOs
{
    public class ProductCreateDto
    {
        public string Sku { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }
    }
}