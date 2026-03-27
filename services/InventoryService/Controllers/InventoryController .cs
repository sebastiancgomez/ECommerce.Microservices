using InventoryService.Services;
using InventoryService.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
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
            await _inventoryService.CreateInventoryAsync(dto);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
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
        var result = await _inventoryService.ReserveStockAsync(dto);

        if (!result)
            return Conflict("Not enough stock");

        return Ok();
    }

    [HttpPost("release")]
    public async Task<IActionResult> Release(ReleaseStockDto dto)
    {
        var result = await _inventoryService.ReleaseStockAsync(dto);

        if (!result)
            return NotFound();

        return Ok();
    }
}