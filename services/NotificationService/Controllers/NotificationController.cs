using Microsoft.AspNetCore.Mvc;
using NotificationService.Models;
using NotificationService.Services;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly NotificationSender _sender;

    public NotificationController(NotificationSender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> SendNotification([FromBody] Notification notification)
    {
        await _sender.SendNotificationAsync(notification.Recipient, notification.Message);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var notifications = await _sender.GetSentNotificationsAsync();
        return Ok(notifications);
    }
}
