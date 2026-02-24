using Microsoft.EntityFrameworkCore;
using PricingService.Data;
using PricingService.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PricingDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("PricingDB")
    ));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PricingDbContext>();

    db.Database.Migrate();

    if (!db.PricingRules.Any())
    {
        db.PricingRules.AddRange(
            new PricingRule(1, 100m),
            new PricingRule(2, 50m),
            new PricingRule(3, 200m)
        );

        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();