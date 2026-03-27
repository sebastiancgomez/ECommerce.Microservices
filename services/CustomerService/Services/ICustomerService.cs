using CustomerService.DTOs;

namespace CustomerService.Services;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllAsync();
    Task<CustomerDto?> GetByIdAsync(int id);
    Task<CustomerDto> CreateAsync(CreateCustomerDto dto);
    Task<bool> UpdateAsync(int id, CreateCustomerDto dto);
    Task<bool> DeleteAsync(int id);
}