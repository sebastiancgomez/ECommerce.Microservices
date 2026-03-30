using InventoryService.DTOs;


namespace InventoryService.Services;

public interface IInventoryService
{
    Task CreateInventoryAsync(CreateInventoryDto dto);
    Task<InventoryDto?> GetInventoryAsync(int productId);
    Task AddStockAsync(AddStockDto dto);
    Task ReserveStockAsync(ReserveStockDto dto);
    Task ReleaseStockAsync(ReleaseStockDto dto);
}