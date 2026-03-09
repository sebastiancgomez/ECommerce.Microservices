public class UpdateProductDto
{
    public string Name { get; set; }
    public string Sku { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}