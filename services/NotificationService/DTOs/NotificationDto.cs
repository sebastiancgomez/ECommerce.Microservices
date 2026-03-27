namespace NotificationService.DTOs;

public class NotificationDto
{
    public int Id { get; set; }
    public string Recipient { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}