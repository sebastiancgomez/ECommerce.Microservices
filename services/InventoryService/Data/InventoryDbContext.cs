using InventoryService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace InventoryService.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options) { }

    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
}