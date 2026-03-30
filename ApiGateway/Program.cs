var builder = WebApplication.CreateBuilder(args);

// YARP
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Logging (ya alineado con tu estilo)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Middleware base
app.UseRouting();

app.Use(async (context, next) =>
{
    context.Request.Headers["X-Request-Id"] = Guid.NewGuid().ToString();
    await next();
});

// Health check del gateway
app.MapGet("/health", () => Results.Ok("Gateway is healthy"));

// Proxy
app.MapReverseProxy();

app.Run();