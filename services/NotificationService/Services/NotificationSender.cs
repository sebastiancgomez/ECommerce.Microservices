using NotificationService.Models;

namespace NotificationService.Services;

public class NotificationSender
{
    private readonly List<Notification> _sentNotifications = new();

    public Task SendNotificationAsync(string recipient, string message)
    {
        var notification = new Notification
        {
            Recipient = recipient,
            Message = message
        };

        _sentNotifications.Add(notification);

        // Simula envío (puedes reemplazar con email, RabbitMQ, etc.)
        Console.WriteLine($"Notification sent to {recipient}: {message}");

        return Task.CompletedTask;
    }

    public Task<List<Notification>> GetSentNotificationsAsync()
    {
        return Task.FromResult(_sentNotifications);
    }
}
