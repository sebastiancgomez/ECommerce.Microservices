using CustomerService.DTOs;
using CustomerService.Models;
using CustomerService.Repositories;

namespace CustomerService.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(ICustomerRepository repository, ILogger<CustomerService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all customers");
        var customers = await _repository.GetAllAsync();
        _logger.LogInformation("Retrieved {Count} customers", customers.Count());
        return customers.Select(ToDto);
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving customer with ID {CustomerId}", id);
        var customer = await _repository.GetByIdAsync(id);
        _logger.LogInformation(customer is null
            ? "Customer with ID {CustomerId} not found"
            : "Customer with ID {CustomerId} retrieved successfully", id);
        return customer is null ? null : ToDto(customer);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
    {
        _logger.LogInformation("Creating new customer with email {Email}", dto.Email);
        var existing = await _repository.GetByEmailAsync(dto.Email);
        if (existing is not null)
        {
            _logger.LogWarning("Customer with email {Email} already exists", dto.Email);
            throw new InvalidOperationException($"A customer with email {dto.Email} already exists.");
        }

        var customer = new Customer
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email
        };

        await _repository.AddAsync(customer);
        _logger.LogInformation("Customer with email {Email} created successfully with ID {CustomerId}", dto.Email, customer.Id);
        return ToDto(customer);
    }

    public async Task<bool> UpdateAsync(int id, CreateCustomerDto dto)
    {
        _logger.LogInformation("Updating customer with ID {CustomerId}", id);
        var customer = await _repository.GetByIdAsync(id);
        if (customer is null)
        {
            _logger.LogWarning("Customer with ID {CustomerId} not found for update", id);
            return false;
        }

        customer.FirstName = dto.FirstName;
        customer.LastName = dto.LastName;
        customer.Email = dto.Email;

        await _repository.UpdateAsync(customer);
        _logger.LogInformation("Customer with ID {CustomerId} updated successfully", id);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting customer with ID {CustomerId}", id);
        var customer = await _repository.GetByIdAsync(id);
        if (customer is null)
        {
            _logger.LogWarning("Customer with ID {CustomerId} not found for deletion", id);
            return false;
        }


        await _repository.DeleteAsync(customer);
        _logger.LogInformation("Customer with ID {CustomerId} deleted successfully", id);
        return true;
    }

    private static CustomerDto ToDto(Customer c) => new()
    {
        Id = c.Id,
        FirstName = c.FirstName,
        LastName = c.LastName,
        Email = c.Email,
        CreatedAt = c.CreatedAt
    };
}