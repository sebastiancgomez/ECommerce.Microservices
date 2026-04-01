using System.Text;
using System.Text.Json;
using PaymentService.Models;
using PaymentService.Messaging;
using RabbitMQ.Client;

public class PaymentEventPublisher : IPaymentEventPublisher
{
    private readonly IConfiguration _configuration;

    public PaymentEventPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task PublishPaymentCompletedAsync(Payment payment)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQ:Host"],
            Port = int.Parse(_configuration["RabbitMQ:Port"]!),
            UserName = _configuration["RabbitMQ:Username"],
            Password = _configuration["RabbitMQ:Password"]
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "payment-completed",
            durable: false,
            exclusive: false,
            autoDelete: false);

        var message = JsonSerializer.Serialize(payment);
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(
            exchange: "",
            routingKey: "payment-completed",
            basicProperties: null,
            body: body);

        return Task.CompletedTask;
    }
}