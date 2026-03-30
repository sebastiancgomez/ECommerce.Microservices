using InventoryService.Clients;
using InventoryService.DTOs;
using InventoryService.Models;
using InventoryService.Repositories;
using Polly.CircuitBreaker;
using Polly.Timeout;
namespace InventoryService.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _repository;
    private readonly IProductClient _productClient;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(IInventoryRepository repository, IProductClient productClient, ILogger<InventoryService> logger)
    {
        _repository = repository;
        _productClient = productClient;
        _logger = logger;
    }

    public async Task CreateInventoryAsync(CreateInventoryDto dto)
    {
        try
        {
            var productExists = await _productClient.ExistsAsync(dto.ProductId);
            if (!productExists)
            {
                _logger.LogWarning("Attempted to create inventory for non-existent product {ProductId}", dto.ProductId);
                throw new InvalidOperationException($"Product {dto.ProductId} does not exist.");
            }

            var item = new InventoryItem(dto.ProductId, dto.Stock);

            await _repository.AddAsync(item);
            _logger.LogInformation("Created inventory for product {ProductId} with stock {Stock}", dto.ProductId, dto.Stock);
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(ex, "[CIRCUIT BREAKER] Circuit is open, service unavailable. Product {ProductId} with stock {Stock}",
                dto.ProductId, dto.Stock);

            throw; // el controlador lo captura y retorna 503
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex, "[TIMEOUT] Service timed out. Product {ProductId} with stock {Stock}",
                dto.ProductId, dto.Stock);

            throw;
        }
        catch
        {
            _logger.LogError("Inventory creation failed for product {ProductId} with stock {Stock}",
                dto.ProductId, dto.Stock);

            throw;
        }
    }

    public async Task<InventoryDto?> GetInventoryAsync(int productId)
    {
        try
        {
            _logger.LogInformation("Retrieving inventory for product {ProductId}", productId);
            var item = await _repository.GetByProductIdAsync(productId);

            if (item == null)
            {
                _logger.LogWarning("Inventory not found for product {ProductId}", productId);
                return null;
            }
            _logger.LogInformation("Inventory found for product {ProductId} with stock {Stock}", productId, item.AvailableStock);
            return new InventoryDto
            {
                ProductId = item.ProductId,
                AvailableStock = item.AvailableStock,
                ReservedStock = item.ReservedStock,
                TotalStock = item.TotalStock
            };
        }
        catch(Exception ex)
        {
            _logger.LogError(ex,"Failed to retrieve inventory for product {ProductId}.", productId);
            throw;
        }

    }

    public async Task AddStockAsync(AddStockDto dto)
    {
        try
        {
            _logger.LogInformation("Adding {Quantity} stock to product {ProductId}", dto.Quantity, dto.ProductId);

            var productExists = await _productClient.ExistsAsync(dto.ProductId);
            if (!productExists)
            {
                _logger.LogWarning("Attempted to add stock to non-existent product {ProductId}", dto.ProductId);
                throw new InvalidOperationException("Product does not exist");
            }

            var item = await _repository.GetByProductIdAsync(dto.ProductId);

            if (item == null)
            {
                _logger.LogWarning("Inventory not found for product {ProductId}", dto.ProductId);
                throw new InvalidOperationException("Inventory not found");
            }

            item.AddStock(dto.Quantity);

            await _repository.UpdateAsync(item);

            _logger.LogInformation("Stock updated for product {ProductId}. New stock: {Stock}",
                dto.ProductId, item.AvailableStock);
        }         
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(ex, "[CIRCUIT BREAKER] Product service unavailable for Product {ProductId}", dto.ProductId);
            throw;
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex, "[TIMEOUT] Product service timeout for Product {ProductId}", dto.ProductId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add stock for product {ProductId}.", dto.ProductId);
            throw;
        }
    }

    public async Task ReserveStockAsync(ReserveStockDto dto)
    {
        try
        {
            _logger.LogInformation("Attempting to reserve {Quantity} stock for product {ProductId}", dto.Quantity, dto.ProductId);
            var item = await _repository.GetByProductIdAsync(dto.ProductId);

            if (item == null)
            {
                _logger.LogWarning("Inventory not found for product {ProductId}", dto.ProductId);
                throw new InvalidOperationException($"Inventory not found for product {dto.ProductId}");
            }


            item.Reserve(dto.Quantity);

            await _repository.UpdateAsync(item);
            _logger.LogInformation("Successfully reserved {Quantity} stock for product {ProductId}. Remaining stock: {Stock}", dto.Quantity, dto.ProductId, item.AvailableStock);

        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(ex, "[CIRCUIT BREAKER] Product service unavailable for Product {ProductId}", dto.ProductId);
            throw;
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex, "[TIMEOUT] Product service timeout for Product {ProductId}", dto.ProductId);
            throw;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to reserve {Quantity} stock for product {ProductId}. Not enough stock available.", dto.Quantity, dto.ProductId);
            throw;
        }
    }
    public async Task ReleaseStockAsync(ReleaseStockDto dto)
    {
        try
        {
            _logger.LogInformation("Attempting to release {Quantity} stock for product {ProductId}", dto.Quantity, dto.ProductId);
            var item = await _repository.GetByProductIdAsync(dto.ProductId);

            if (item == null)
            {
                _logger.LogWarning("Inventory not found for product {ProductId}", dto.ProductId);
                throw new InvalidOperationException($"Inventory not found for product {dto.ProductId}");
            }

            item.Release(dto.Quantity);

            await _repository.UpdateAsync(item);
            _logger.LogInformation("Successfully released {Quantity} stock for product {ProductId}. New stock: {Stock}", dto.Quantity, dto.ProductId, item.AvailableStock);
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(ex, "[CIRCUIT BREAKER] Product service unavailable for Product {ProductId}", dto.ProductId);
            throw;
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex, "[TIMEOUT] Product service timeout for Product {ProductId}", dto.ProductId);
            throw;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to release {Quantity} stock for product {ProductId}. Not enough reserved stock to release.", dto.Quantity, dto.ProductId);
            throw;
        }
    }
}

