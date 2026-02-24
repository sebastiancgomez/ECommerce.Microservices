using Microsoft.EntityFrameworkCore;
using OrderService.Clients;
using OrderService.Data;

var builder = WebApplication.CreateBuilder(args);

// Obtener la cadena de conexión desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("OrderDb");

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddHttpClient<ProductClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5100/");
});

builder.Services.AddHttpClient<InventoryClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5600/");
});

builder.Services.AddHttpClient<PricingClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5700/");
});

// Resto de configuración...
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
