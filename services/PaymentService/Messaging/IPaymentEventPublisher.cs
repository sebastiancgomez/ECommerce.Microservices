using PaymentService.Models;

namespace PaymentService.Messaging;

public interface IPaymentEventPublisher
{
    Task PublishPaymentCompletedAsync(Payment payment);
}
