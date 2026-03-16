using InventoryService.Models;

namespace InventoryService.Repositories;

public interface IInventoryRepository
{
    Task<InventoryItem?> GetByProductIdAsync(int productId);
    Task AddAsync(InventoryItem item);
    Task UpdateAsync(InventoryItem item);
}