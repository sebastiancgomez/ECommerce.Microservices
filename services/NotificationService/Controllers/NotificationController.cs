using Microsoft.AspNetCore.Mvc;
using NotificationService.DTOs;
using NotificationService.Services;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationSender _sender;

    public NotificationController(INotificationSender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationDto dto)
    {
        await _sender.SendAsync(dto.Recipient, dto.Message);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var notifications = await _sender.GetAllAsync();
        return Ok(notifications);
    }
}