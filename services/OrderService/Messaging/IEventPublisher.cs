namespace OrderService.Messaging;

public interface IEventPublisher
{
    void Publish<T>(T @event, string queueName);
}