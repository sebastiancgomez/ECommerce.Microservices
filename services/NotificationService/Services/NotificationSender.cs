using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.DTOs;
using NotificationService.Models;

namespace NotificationService.Services;

public class NotificationSender : INotificationSender
{
    private readonly NotificationDbContext _context;
    private readonly ILogger<NotificationSender> _logger;

    public NotificationSender(NotificationDbContext context, ILogger<NotificationSender> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SendAsync(string recipient, string message)
    {
        var notification = new Notification
        {
            Recipient = recipient,
            Message = message
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Notification sent to {recipient}: {message}");
    }

    public async Task<IEnumerable<NotificationDto>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all notifications");
        var notifications = await _context.Notifications
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        _logger.LogInformation($"Get {notifications.Count} Notifications");
        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Recipient = n.Recipient,
            Message = n.Message,
            CreatedAt = n.CreatedAt
        });
    }
}