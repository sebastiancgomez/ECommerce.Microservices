using OrderService.Clients;
using OrderService.Models;
using OrderService.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OrderService.Messaging;

public class PaymentResultConsumer : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PaymentResultConsumer> _logger;

    private IConnection? _connection;
    private IModel? _channel;

    private const string CompletedQueue = "payment.completed";
    private const string FailedQueue = "payment.failed";

    public PaymentResultConsumer(
        IConfiguration config,
        IServiceScopeFactory scopeFactory,
        ILogger<PaymentResultConsumer> logger)
    {
        _config = config;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _config["RabbitMQ:Host"] ?? "localhost",
            Port = int.Parse(_config["RabbitMQ:Port"] ?? "5672"),
            UserName = _config["RabbitMQ:Username"] ?? "guest",
            Password = _config["RabbitMQ:Password"] ?? "guest",
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(CompletedQueue, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueDeclare(FailedQueue, durable: true, exclusive: false, autoDelete: false);
        _channel.BasicQos(0, 1, false);

        _logger.LogInformation("[RabbitMQ] PaymentResultConsumer listening on '{C}' and '{F}'",
            CompletedQueue, FailedQueue);

        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Consumer para payment.completed
        var completedConsumer = new AsyncEventingBasicConsumer(_channel);
        completedConsumer.Received += async (_, ea) =>
            await HandleAsync(ea, OrderStatus.Paid, options, compensate: false);
        _channel!.BasicConsume(CompletedQueue, autoAck: false, completedConsumer);

        // Consumer para payment.failed
        var failedConsumer = new AsyncEventingBasicConsumer(_channel);
        failedConsumer.Received += async (_, ea) =>
            await HandleAsync(ea, OrderStatus.Cancelled, options, compensate: true);
        _channel!.BasicConsume(FailedQueue, autoAck: false, failedConsumer);

        return Task.CompletedTask;
    }

    private async Task HandleAsync(
        BasicDeliverEventArgs ea,
        OrderStatus targetStatus,
        JsonSerializerOptions options,
        bool compensate)
    {
        var json = Encoding.UTF8.GetString(ea.Body.ToArray());

        try
        {
            // El payload es el modelo Payment de PaymentService
            var payload = JsonSerializer.Deserialize<PaymentEventPayload>(json, options);
            if (payload is null)
            {
                _channel!.BasicNack(ea.DeliveryTag, false, requeue: false);
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            var inventoryClient = scope.ServiceProvider.GetRequiredService<IInventoryClient>();

            var order = await repository.GetByIdAsync(payload.OrderId);
            if (order is null)
            {
                _logger.LogWarning("[PaymentConsumer] Order {OrderId} not found", payload.OrderId);
                _channel!.BasicNack(ea.DeliveryTag, false, requeue: false);
                return;
            }

            // Saga de compensación si el pago falló
            if (compensate)
            {
                _logger.LogWarning("[SAGA] Payment failed for Order {OrderId}, releasing stock", payload.OrderId);

                foreach (var item in order.Items)
                {
                    try
                    {
                        await inventoryClient.Release(item.ProductId, item.Quantity);
                        _logger.LogInformation("[SAGA] Released {Qty} units of product {ProductId}",
                            item.Quantity, item.ProductId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[SAGA] Failed to release stock for product {ProductId}",
                            item.ProductId);
                    }
                }
            }

            order.ChangeStatus(targetStatus);
            await repository.SaveChangesAsync();

            _logger.LogInformation(
                "[PaymentConsumer] Order {OrderId} → {Status} (ResultCode={ResultCode})",
                payload.OrderId, targetStatus, payload.ResultCode);

            _channel!.BasicAck(ea.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[PaymentConsumer] Error processing payment event");
            _channel!.BasicNack(ea.DeliveryTag, false, requeue: true);
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}

// DTO mínimo para deserializar el evento de PaymentService
public class PaymentEventPayload
{
    public int OrderId { get; init; }
    public string Status { get; init; } = string.Empty;
    public string ResultCode { get; init; } = string.Empty;
}