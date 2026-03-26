using Microsoft.EntityFrameworkCore;
using PricingService.Models;

namespace PricingService.Data;

public class PricingDbContext : DbContext
{
    public PricingDbContext(DbContextOptions<PricingDbContext> options)
        : base(options) { }

    public DbSet<PricingRule> PricingRules => Set<PricingRule>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PricingRule>()
            .Property(p => p.DiscountPercentage)
            .HasPrecision(18, 2); // Precisión segura para SQL Server
    }
}