using OrderService.DTOs;

namespace OrderService.Services;

public interface IOrderService
{
    Task<OrderResponseDto> CreateAsync(CreateOrderDto dto);
    Task<OrderResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<OrderResponseDto>> GetByCustomerIdAsync(int customerId);
}