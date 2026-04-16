using OrderService.Clients;
using OrderService.DTOs;
using OrderService.Events;
using OrderService.Messaging;
using OrderService.Models;
using OrderService.Repositories;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace OrderService.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly IProductClient _productClient;
    private readonly IInventoryClient _inventoryClient;
    private readonly IPricingClient _pricingClient;
    private readonly IPaymentClient _paymentClient;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository repository,
        IProductClient productClient,
        IInventoryClient inventoryClient,
        IPricingClient pricingClient,
        IPaymentClient paymentClient,
        IEventPublisher eventPublisher,
        ILogger<OrderService> logger)
    {
        _repository = repository;
        _productClient = productClient;
        _inventoryClient = inventoryClient;
        _pricingClient = pricingClient;
        _paymentClient = paymentClient;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<CreateOrderResponseDto> CreateAsync(CreateOrderDto dto)
    {
        _logger.LogInformation("Creating order for customer {CustomerId} with {ItemCount} items",
            dto.CustomerId, dto.Items.Count);
        var order = new Order(dto.CustomerId);
        var reservedItems = new List<(int ProductId, int Quantity)>(); // ← track de reservas

        try
        {
            foreach (var itemDto in dto.Items)
            {
                // 1. Validar que el producto existe
                var product = await _productClient.GetProductById(itemDto.ProductId);
                if (product is null)
                {
                    _logger.LogWarning("Product {ProductId} not found", itemDto.ProductId);
                    throw new InvalidOperationException($"Product {itemDto.ProductId} not found.");
                }

                // 2. Calcular precio
                var pricingRequest = new PricingRequestDto
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    BasePrice = product.Price
                };
                var pricingInfo = await _pricingClient.GetPrice(pricingRequest);
                _logger.LogInformation("Pricing for product {ProductId}: base={BasePrice} final={FinalPrice}",
                    itemDto.ProductId, product.Price, pricingInfo.FinalPrice);

                // 3. Validar stock
                var available = await _inventoryClient.IsAvailable(itemDto.ProductId, itemDto.Quantity);
                if (!available)
                {
                    _logger.LogWarning("Not enough stock for product {ProductId}, quantity requested {Quantity}",
                        itemDto.ProductId, itemDto.Quantity);
                    throw new InvalidOperationException($"Not enough stock for product {itemDto.ProductId}.");
                }

                // 4. Reservar stock
                await _inventoryClient.Reserve(itemDto.ProductId, itemDto.Quantity);
                reservedItems.Add((itemDto.ProductId, itemDto.Quantity)); // ← registrar reserva
                _logger.LogInformation("Reserved {Quantity} units of product {ProductId}",
                   itemDto.Quantity, itemDto.ProductId);

                // 5. Agregar item a la orden
                order.AddItem(itemDto.ProductId, product.Name, pricingInfo.FinalPrice, itemDto.Quantity);
            }

            order.ChangeStatus(OrderStatus.PendingPayment);
            await _repository.AddAsync(order);

            _logger.LogInformation("Order {OrderId} created in Pending state for customer {CustomerId}",
           order.Id, order.CustomerId);

            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                Total = order.Total,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(i => new OrderCreatedEventItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
            _eventPublisher.Publish(orderCreatedEvent, "order.created");
            _logger.LogInformation("Order {OrderId} created successfully for customer {CustomerId} with total {Total}",
                order.Id, order.CustomerId, order.Total);

            return new CreateOrderResponseDto
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                Total = order.Total,
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedAt,
                PaymentUrl = $"/checkout?orderId={order.Id}", // la UI redirige aquí
                Items = order.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            };
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex, "[TIMEOUT] Service timed out. Customer {CustomerId}", dto.CustomerId);

            await CompensateAsync(reservedItems);

            throw;
        }
        catch
        {
            _logger.LogError("Order creation failed for customer {CustomerId}, starting Saga compensation",
                dto.CustomerId);

            await CompensateAsync(reservedItems);

            throw;
        }
    }

    public async Task<OrderResponseDto?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Fetching order {OrderId}", id);
        var order = await _repository.GetByIdAsync(id);
        if (order is null)
            _logger.LogWarning("Order {OrderId} not found", id);
        return order is null ? null : ToDto(order);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetByCustomerIdAsync(int customerId)
    {
        _logger.LogInformation("Fetching orders for customer {CustomerId}", customerId);
        var orders = await _repository.GetByCustomerIdAsync(customerId);
        return orders.Select(ToDto);
    }
    private async Task CompensateAsync(List<(int ProductId, int Quantity)> reservedItems)
    {
        foreach (var (productId, quantity) in reservedItems)
        {
            try
            {
                await _inventoryClient.Release(productId, quantity);
                _logger.LogInformation("[SAGA] Released {Quantity} units of product {ProductId}",
                    quantity, productId);
            }
            catch (Exception releaseEx)
            {
                _logger.LogError(releaseEx, "[SAGA] Failed to release stock for product {ProductId}",
                    productId);
            }
        }
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