using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Clients;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly ProductClient _productClient;
    private readonly OrderDbContext _db;
    private readonly InventoryClient _inventory;
    private readonly PricingClient _pricing;

    public OrderController( OrderDbContext db, 
        InventoryClient inventory,
        PricingClient pricing,
        ProductClient productClient)
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
            return BadRequest("Order must contain at least one item.");

        var order = new Order();

        foreach (var itemDto in dto.Items)
        {
            // 1️ Obtener información de pricing
            var product = await _productClient.GetProductById(itemDto.ProductId);
           

            if (product == null)
                return BadRequest($"Cannot get pricing for product {itemDto.ProductId}");
            var pricingInfo = await _pricing.GetPrice(itemDto.ProductId);
            // 2️ Verificar disponibilidad
            var available = await _inventory.IsAvailable(
                itemDto.ProductId,
                itemDto.Quantity);

            if (!available)
                return BadRequest($"Not enough stock for product {itemDto.ProductId}");

            // 3️ Reservar stock
            await _inventory.Reserve(itemDto.ProductId, itemDto.Quantity);

            // 4️ Agregar item a la orden
            order.AddItem(
                itemDto.ProductId,
                product.Name,
                pricingInfo,
                itemDto.Quantity);
        }

        // 5️ Cambiar estado
        order.ChangeStatus(OrderStatus.Confirmed);

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetOrderById),
            new { id = order.Id },
            order);
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
