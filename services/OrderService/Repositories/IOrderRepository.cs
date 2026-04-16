using OrderService.Models;

namespace OrderService.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId);
    Task AddAsync(Order order); 
    Task SaveChangesAsync();
}