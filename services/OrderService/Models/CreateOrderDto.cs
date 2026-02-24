namespace OrderService.Models;

public class CreateOrderDto
{
    public List<CreateOrderItemDto> Items { get; set; } = new();
}


