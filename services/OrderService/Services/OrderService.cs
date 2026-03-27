using OrderService.Clients;
using OrderService.DTOs;
using OrderService.Models;
using OrderService.Repositories;

namespace OrderService.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly IProductClient _productClient;
    private readonly IInventoryClient _inventoryClient;
    private readonly IPricingClient _pricingClient;

    public OrderService(
        IOrderRepository repository,
        IProductClient productClient,
        IInventoryClient inventoryClient,
        IPricingClient pricingClient)
    {
        _repository = repository;
        _productClient = productClient;
        _inventoryClient = inventoryClient;
        _pricingClient = pricingClient;
    }

    public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto)
    {
        var order = new Order(dto.CustomerId);
        var reservedItems = new List<(int ProductId, int Quantity)>(); // ← track de reservas

        try
        {
            foreach (var itemDto in dto.Items)
            {
                // 1. Validar que el producto existe
                var product = await _productClient.GetProductById(itemDto.ProductId);
                if (product is null)
                    throw new InvalidOperationException($"Product {itemDto.ProductId} not found.");

                // 2. Calcular precio
                var pricingRequest = new PricingRequestDto
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    BasePrice = product.Price
                };
                var pricingInfo = await _pricingClient.GetPrice(pricingRequest);

                // 3. Validar stock
                var available = await _inventoryClient.IsAvailable(itemDto.ProductId, itemDto.Quantity);
                if (!available)
                    throw new InvalidOperationException($"Not enough stock for product {itemDto.ProductId}.");

                // 4. Reservar stock
                await _inventoryClient.Reserve(itemDto.ProductId, itemDto.Quantity);
                reservedItems.Add((itemDto.ProductId, itemDto.Quantity)); // ← registrar reserva

                // 5. Agregar item a la orden
                order.AddItem(itemDto.ProductId, product.Name, pricingInfo.FinalPrice, itemDto.Quantity);
            }

            order.ChangeStatus(OrderStatus.Confirmed);
            await _repository.AddAsync(order);

            return ToDto(order);
        }
        catch
        {
            // SAGA COMPENSACIÓN: liberar todo lo que ya se reservó
            foreach (var (productId, quantity) in reservedItems)
            {
                try
                {
                    await _inventoryClient.Release(productId, quantity);
                }
                catch (Exception releaseEx)
                {
                    // Loguear pero no relanzar, la compensación no debe ocultar el error original
                    Console.WriteLine($"[SAGA] Failed to release stock for product {productId}: {releaseEx.Message}");
                }
            }

            throw; // relanza la excepción original
        }
    }

    public async Task<OrderResponseDto?> GetByIdAsync(int id)
    {
        var order = await _repository.GetByIdAsync(id);
        return order is null ? null : ToDto(order);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetByCustomerIdAsync(int customerId)
    {
        var orders = await _repository.GetByCustomerIdAsync(customerId);
        return orders.Select(ToDto);
    }

    private static OrderResponseDto ToDto(Order order) => new()
    {
        Id = order.Id,
        CustomerId = order.CustomerId,
        CreatedAt = order.CreatedAt,
        Status = order.Status.ToString(),
        Total = order.Total,
        Items = order.Items.Select(i => new OrderItemDto
        {
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            UnitPrice = i.UnitPrice,
            Quantity = i.Quantity
        }).ToList()
    };
}