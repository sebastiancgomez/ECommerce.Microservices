using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly ProductServiceClient _productClient;
    private readonly OrderDbContext _db;
    private readonly InventoryServiceClient _inventory;
    private readonly PricingServiceClient _pricing;

    public OrderController( OrderDbContext db, 
        InventoryServiceClient inventory,
        PricingServiceClient pricing,
        ProductServiceClient productClient)
    {
        _db = db;
        _inventory = inventory;
        _pricing = pricing;
        _productClient = productClient;
    }

    // GET /api/Order/products (ya lo tienes)
    [HttpGet("products")]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productClient.GetProductsAsync();
        return Ok(products);
    }

    // POST /api/Order
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
            return BadRequest("No products provided.");

        var order = new Order();

        foreach (var itemDto in dto.Items)
        {
            // Validar stock en InventoryService
            var available = await _inventory.IsAvailable(itemDto.ProductId, itemDto.Quantity);
            if (!available)
                return BadRequest($"Product {itemDto.ProductId} does not have enough stock.");

            // Obtener precio final desde PricingService
            var pricingInfo = await _pricing.GetPriceAsync(itemDto.ProductId);
            if (pricingInfo == null)
                return BadRequest($"Cannot get pricing for product {itemDto.ProductId}");

            order.AddItem(itemDto.ProductId, pricingInfo.Name, pricingInfo.FinalPrice, itemDto.Quantity);
        }

        // Confirmar orden (stock y precio verificados)
        order.ChangeStatus(OrderStatus.Confirmed);

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
    }

    // GET /api/Order/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();

        return Ok(order);
    }
}
