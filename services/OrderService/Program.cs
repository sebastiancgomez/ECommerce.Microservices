using Microsoft.EntityFrameworkCore;
using OrderService.Clients;
using OrderService.Data;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<InventoryClient>(client =>
{
    client.BaseAddress = new Uri("http://inventoryservice:8080");
});
builder.Services.AddHttpClient<ProductClient>(client =>
{
    client.BaseAddress = new Uri("http://ProductService:8080");
});
builder.Services.AddHttpClient<PricingClient>(client =>
{
    client.BaseAddress = new Uri("http://PricingService:8080");
});

var app = builder.Build();

// Ejecutar migraciones automáticamente
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();