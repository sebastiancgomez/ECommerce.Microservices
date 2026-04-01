using Microsoft.EntityFrameworkCore;
using PaymentService.Models;
using System.Collections.Generic;

namespace PaymentService.Data;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
        : base(options) { }

    public DbSet<Payment> Payments => Set<Payment>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            entity.Property(p => p.Currency)
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");

            entity.Property(p => p.Status)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(p => p.Method)
                .HasMaxLength(20)
                .IsRequired();
        });
    }
}