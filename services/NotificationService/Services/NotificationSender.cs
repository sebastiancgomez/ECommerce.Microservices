using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.DTOs;
using NotificationService.Models;

namespace NotificationService.Services;

public class NotificationSender : INotificationSender
{
    private readonly NotificationDbContext _context;

    public NotificationSender(NotificationDbContext context)
    {
        _context = context;
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

        Console.WriteLine($"Notification sent to {recipient}: {message}");
    }

    public async Task<IEnumerable<NotificationDto>> GetAllAsync()
    {
        var notifications = await _context.Notifications
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Recipient = n.Recipient,
            Message = n.Message,
            CreatedAt = n.CreatedAt
        });
    }
}