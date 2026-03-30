using System.Text;
using System.Text.Json;
using NotificationService.Events;
using NotificationService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationService.Messaging;

public class OrderCreatedConsumer : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    private IConnection? _connection;
    private IModel? _channel;

    private const string QueueName = "order.created";

    public OrderCreatedConsumer(
        IConfiguration config,
        IServiceScopeFactory scopeFactory,
        ILogger<OrderCreatedConsumer> logger)
    {
        _config = config;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _config["RabbitMQ:Host"] ?? "localhost",
            Port = int.Parse(_config["RabbitMQ:Port"] ?? "5672"),
            UserName = _config["RabbitMQ:Username"] ?? "guest",
            Password = _config["RabbitMQ:Password"] ?? "guest",
            DispatchConsumersAsync = true
        };

        const int maxRetries = 5;
        var delay = TimeSpan.FromSeconds(5);

        for (int i = 1; i <= maxRetries; i++)
        {
            try
            {
                _logger.LogInformation("[RabbitMQ] Attempt {Attempt} to connect...", i);

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare(
                    queue: QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                _logger.LogInformation("[RabbitMQ] Connected successfully");

                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[RabbitMQ] Connection attempt {Attempt} failed", i);

                if (i == maxRetries)
                    throw;

                await Task.Delay(delay, cancellationToken);
            }
        }

        await base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            _logger.LogInformation("[RabbitMQ] Message received — DeliveryTag={Tag}", ea.DeliveryTag);

            try
            {
                var @event = JsonSerializer.Deserialize<OrderCreatedEvent>(json, options);

                if (@event is null)
                {
                    _logger.LogWarning("[RabbitMQ] Could not deserialize message, discarding");
                    _channel!.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                    return;
                }

                // Usamos scope porque INotificationSender es Scoped (DbContext)
                using var scope = _scopeFactory.CreateScope();
                var sender = scope.ServiceProvider.GetRequiredService<INotificationSender>();

                var recipient = $"customer-{@event.CustomerId}";
                var message = $"Order #{@event.OrderId} confirmed. Total: {@event.Total:C}. " +
                                $"Items: {@event.Items.Count}. Date: {@event.CreatedAt:u}";

                await sender.SendAsync(recipient, message);

                _channel!.BasicAck(ea.DeliveryTag, multiple: false);

                _logger.LogInformation(
                    "[RabbitMQ] OrderCreated processed — OrderId={OrderId} CustomerId={CustomerId}",
                    @event.OrderId, @event.CustomerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[RabbitMQ] Error processing message, requeueing");
                _channel!.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel!.BasicConsume(
            queue: QueueName,
            autoAck: false,   // ACK manual — el mensaje no se pierde si falla
            consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}