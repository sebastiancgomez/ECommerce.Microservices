using InventoryService.DTOs;
using InventoryService.Models;
using InventoryService.Repositories;
namespace InventoryService.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _repository;

    public InventoryService(IInventoryRepository repository)
    {
        _repository = repository;
    }

    public async Task CreateInventoryAsync(CreateInventoryDto dto)
    {
        var item = new InventoryItem(dto.ProductId, dto.Stock);

        await _repository.AddAsync(item);
    }

    public async Task<InventoryDto?> GetInventoryAsync(int productId)
    {
        var item = await _repository.GetByProductIdAsync(productId);

        if (item == null)
            return null;

        return new InventoryDto
        {
            ProductId = item.ProductId,
            Stock = item.Stock
        };
    }

    public async Task<bool> ReserveStockAsync(ReserveStockDto dto)
    {
        var item = await _repository.GetByProductIdAsync(dto.ProductId);

        if (item == null)
            return false;

        try
        {
            item.Reserve(dto.Quantity);

            await _repository.UpdateAsync(item);

            return true;
        }
        catch
        {
            return false;
        }
    }
    public async Task<bool> ReleaseStockAsync(ReleaseStockDto dto)
    {
        var item = await _repository.GetByProductIdAsync(dto.ProductId);

        if (item == null)
            return false;

        item.Release(dto.Quantity);

        await _repository.UpdateAsync(item);

        return true;
    }
}

