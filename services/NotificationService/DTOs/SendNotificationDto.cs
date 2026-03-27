namespace NotificationService.DTOs;

public class SendNotificationDto
{
    public string Recipient { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}