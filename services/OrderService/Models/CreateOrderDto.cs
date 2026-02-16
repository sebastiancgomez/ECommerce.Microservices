namespace OrderService.Models;

public class CreateOrderDto
{
    public List<ProductDto> Products { get; set; } = new();
}


