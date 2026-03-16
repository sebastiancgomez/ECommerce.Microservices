using InventoryService.DTOs;


namespace InventoryService.Services;

public interface IInventoryService
{
    Task CreateInventoryAsync(CreateInventoryDto dto);
    Task<InventoryDto?> GetInventoryAsync(int productId);
    Task<bool> ReserveStockAsync(ReserveStockDto dto);
    Task<bool> ReleaseStockAsync(ReleaseStockDto dto);
}