using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace OrderService.Messaging;

public class RabbitMqEventPublisher : IEventPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMqEventPublisher> _logger;

    public RabbitMqEventPublisher(IConfiguration config, ILogger<RabbitMqEventPublisher> logger)
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
    }

    public void Publish<T>(T @event, string queueName)
    {
        _channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var json = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(json);

        var props = _channel.CreateBasicProperties();
        props.Persistent = true;

        _channel.BasicPublish(
            exchange: "",
            routingKey: queueName,
            basicProperties: props,
            body: body);

        _logger.LogInformation("[RabbitMQ] Published {EventType} to queue '{Queue}'",
            typeof(T).Name, queueName);
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}