using System.Text;
using System.Text.Json;
using PaymentService.Models;
using RabbitMQ.Client;

namespace PaymentService.Messaging;

public class PaymentEventPublisher : IPaymentEventPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<PaymentEventPublisher> _logger;

    private const string CompletedQueue = "payment.completed";
    private const string FailedQueue = "payment.failed";

    public PaymentEventPublisher(IConfiguration config, ILogger<PaymentEventPublisher> logger)
    {
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = config["RabbitMQ:Host"] ?? "localhost",
            Port = int.Parse(config["RabbitMQ:Port"] ?? "5672"),
            UserName = config["RabbitMQ:Username"] ?? "guest",
            Password = config["RabbitMQ:Password"] ?? "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declarar ambas colas al iniciar
        DeclareQueue(CompletedQueue);
        DeclareQueue(FailedQueue);
    }

    public Task PublishPaymentCompletedAsync(Payment payment)
    {
        Publish(payment, CompletedQueue);
        _logger.LogInformation("[RabbitMQ] Published PaymentCompleted for OrderId={OrderId}", payment.OrderId);
        return Task.CompletedTask;
    }

    public Task PublishPaymentFailedAsync(Payment payment)
    {
        Publish(payment, FailedQueue);
        _logger.LogWarning("[RabbitMQ] Published PaymentFailed for OrderId={OrderId}", payment.OrderId);
        return Task.CompletedTask;
    }

    private void Publish(Payment payment, string queue)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payment));
        var props = _channel.CreateBasicProperties();
        props.Persistent = true;

        _channel.BasicPublish(
            exchange: "",
            routingKey: queue,
            basicProperties: props,
            body: body);
    }

    private void DeclareQueue(string name)
    {
        _channel.QueueDeclare(
            queue: name,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}