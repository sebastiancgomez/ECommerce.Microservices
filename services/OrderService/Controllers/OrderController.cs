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

    public OrderController(ProductServiceClient productClient, OrderDbContext db)
    {
        _productClient = productClient;
        _db = db;
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
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        if (dto.Products == null || !dto.Products.Any())
            return BadRequest("No products provided.");

        var order = new Order();

        foreach (var p in dto.Products)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = p.Id,
                ProductName = p.Name,
                ProductPrice = p.Price,
                Quantity = p.Quantity
            });
        }

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
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();

        return Ok(order);
    }
}
