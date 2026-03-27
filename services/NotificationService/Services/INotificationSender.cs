using NotificationService.DTOs;

namespace NotificationService.Services;

public interface INotificationSender
{
    Task SendAsync(string recipient, string message);
    Task<IEnumerable<NotificationDto>> GetAllAsync();
}