using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryService.Data;

namespace InventoryService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly InventoryDbContext _db;

    public InventoryController(InventoryDbContext db)
    {
        _db = db;
    }

    [HttpGet("{productId}/{quantity}")]
    public async Task<bool> IsAvailable(int productId, int quantity)
    {
        var item = await _db.InventoryItems
            .FirstOrDefaultAsync(i => i.ProductId == productId);

        if (item == null) return false;

        return item.Stock >= quantity;
    }

    [HttpGet("Reserve/{productId}/{quantity}")]
    public async Task<bool> Reserve(int productId, int quantity)
    {
        var item = await _db.InventoryItems
            .FirstOrDefaultAsync(i => i.ProductId == productId);

        if (item == null) return false;

        if (item.Stock < quantity) return false;

        item.Reserve(quantity);
        await _db.SaveChangesAsync();

        return true;
    }

    [HttpGet("Release/{productId}/{quantity}")]
    public async Task<bool> Release(int productId, int quantity)
    {
        var item = await _db.InventoryItems
            .FirstOrDefaultAsync(i => i.ProductId == productId);

        if (item == null) return false;

        item.Release(quantity);
        await _db.SaveChangesAsync();

        return true;
    }
}