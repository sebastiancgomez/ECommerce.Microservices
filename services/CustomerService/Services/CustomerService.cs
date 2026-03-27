using CustomerService.DTOs;
using CustomerService.Models;
using CustomerService.Repositories;

namespace CustomerService.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        var customers = await _repository.GetAllAsync();
        return customers.Select(ToDto);
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        var customer = await _repository.GetByIdAsync(id);
        return customer is null ? null : ToDto(customer);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
    {
        var existing = await _repository.GetByEmailAsync(dto.Email);
        if (existing is not null)
            throw new InvalidOperationException($"A customer with email {dto.Email} already exists.");

        var customer = new Customer
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email
        };

        await _repository.AddAsync(customer);
        return ToDto(customer);
    }

    public async Task<bool> UpdateAsync(int id, CreateCustomerDto dto)
    {
        var customer = await _repository.GetByIdAsync(id);
        if (customer is null) return false;

        customer.FirstName = dto.FirstName;
        customer.LastName = dto.LastName;
        customer.Email = dto.Email;

        await _repository.UpdateAsync(customer);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var customer = await _repository.GetByIdAsync(id);
        if (customer is null) return false;

        await _repository.DeleteAsync(customer);
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