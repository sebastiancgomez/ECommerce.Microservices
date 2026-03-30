using InventoryService.DTOs;
using InventoryService.Services;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;

namespace InventoryService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    const string InvalidQuantityMessage = "Quantity must be greater than zero";
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateInventoryDto dto)
    {
        try
        {
            if (dto.Stock <= 0)
                return BadRequest(InvalidQuantityMessage);
            await _inventoryService.CreateInventoryAsync(dto);
            return Ok();
        }
        catch (BrokenCircuitException)
        {
            return ServiceUnavailable();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddStock([FromBody] AddStockDto dto)
    {
        try
        {
            if (dto.Quantity <= 0)
                return BadRequest(InvalidQuantityMessage);
            await _inventoryService.AddStockAsync(dto);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (BrokenCircuitException)
        {
            return ServiceUnavailable();
        }
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> Get(int productId)
    {
        var inventory = await _inventoryService.GetInventoryAsync(productId);

        if (inventory == null)
            return NotFound();

        return Ok(inventory);
    }

    [HttpPost("reserve")]
    public async Task<IActionResult> Reserve(ReserveStockDto dto)
    {
        try
        {
            if (dto.Quantity <= 0)
                return BadRequest(InvalidQuantityMessage);
            await _inventoryService.ReserveStockAsync(dto);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (BrokenCircuitException)
        {
            return ServiceUnavailable();
        }


    }

    [HttpPost("release")]
    public async Task<IActionResult> Release(ReleaseStockDto dto)
    {
        try
        {
            if (dto.Quantity <= 0)
                return BadRequest(InvalidQuantityMessage);
            await   _inventoryService.ReleaseStockAsync(dto);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (BrokenCircuitException)
        {
            return ServiceUnavailable();
        }
    }

    private IActionResult ServiceUnavailable() =>
    StatusCode(503, new
    {
        error = "SERVICE_UNAVAILABLE",
        message = "Product service is currently unavailable."
    });
}