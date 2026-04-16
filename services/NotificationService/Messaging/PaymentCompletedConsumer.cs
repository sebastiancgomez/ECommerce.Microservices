using System.Text;
using System.Text.Json;
using NotificationService.Services;
using NotificationService.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationService.Messaging;

public class PaymentCompletedConsumer : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PaymentCompletedConsumer> _logger;
    private IConnection? _connection;
    private IModel? _channel;

    private const string Queue = "payment.completed";

    public PaymentCompletedConsumer(
        IConfiguration config,
        IServiceScopeFactory scopeFactory,
        ILogger<PaymentCompletedConsumer> logger)
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
        _channel.QueueDeclare(Queue, durable: true, exclusive: false, autoDelete: false);
        _channel.BasicQos(0, 1, false);
        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            try
            {
                var payload = JsonSerializer.Deserialize<PaymentNotificationPayload>(json, options);
                if (payload is null) { _channel!.BasicNack(ea.DeliveryTag, false, false); return; }

                using var scope = _scopeFactory.CreateScope();
                var sender = scope.ServiceProvider.GetRequiredService<INotificationSender>();

                await sender.SendAsync(
                    recipient: $"customer-{payload.CustomerId ?? payload.OrderId}",
                    message: $"✅ Payment confirmed for Order #{payload.OrderId}. Amount: {payload.Amount:C}.");

                _channel!.BasicAck(ea.DeliveryTag, false);
                _logger.LogInformation("[Notification] PaymentCompleted sent for OrderId={OrderId}", payload.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Notification] Error processing PaymentCompleted");
                _channel!.BasicNack(ea.DeliveryTag, false, requeue: true);
            }
        };
        _channel!.BasicConsume(Queue, autoAck: false, consumer);
        return Task.CompletedTask;
    }

    public override void Dispose() { _channel?.Close(); _connection?.Close(); base.Dispose(); }
}