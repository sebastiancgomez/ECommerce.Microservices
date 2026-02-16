namespace NotificationService.Models;

public class Notification
{
    public int Id { get; set; }
    public string Recipient { get; set; } = string.Empty; // Email o UserId
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
