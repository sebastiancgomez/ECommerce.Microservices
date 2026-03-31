using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

// YARP
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Logging (ya alineado con tu estilo)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter();
    })
    .ConfigureResource(resource =>
        resource.AddService(builder.Environment.ApplicationName));

var app = builder.Build();

app.MapPrometheusScrapingEndpoint();

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