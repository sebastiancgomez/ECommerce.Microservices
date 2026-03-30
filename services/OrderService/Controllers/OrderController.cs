using Microsoft.AspNetCore.Mvc;
using OrderService.DTOs;
using OrderService.Services;
using Polly.Timeout;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        try
        {
            var order = await _orderService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }
        catch (TimeoutRejectedException)
        {
            return StatusCode(503, new
            {
                error = "SERVICE_TIMEOUT",
                message = "A service took too long to respond. Please try again later."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order is null) return NotFound();
        return Ok(order);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetByCustomer(int customerId)
    {
        var orders = await _orderService.GetByCustomerIdAsync(customerId);
        return Ok(orders);
    }
}