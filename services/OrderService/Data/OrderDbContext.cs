using Microsoft.EntityFrameworkCore;
using OrderService.Models;
using System.Collections.Generic;

namespace OrderService.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.ProductPrice)
            .HasColumnType("decimal(18,2)"); // Ajusta precisión y escala según necesites
    }
}
