using InventoryService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace InventoryService.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options) { }

    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InventoryItem>()
            .HasIndex(i => i.ProductId)
            .IsUnique();
    }
}