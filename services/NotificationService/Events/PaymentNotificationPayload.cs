namespace NotificationService.Events;

public class PaymentNotificationPayload
{
    public int OrderId { get; init; }
    public int? CustomerId { get; init; }
    public decimal Amount { get; init; }
    public string ResultCode { get; init; } = string.Empty;
}
