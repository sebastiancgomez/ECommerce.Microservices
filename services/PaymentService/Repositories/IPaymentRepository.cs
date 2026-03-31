namespace PaymentService.Repositories;
using PaymentService.Models;

public interface IPaymentRepository
{
    Task<Payment> AddAsync(Payment payment);
    Task<Payment?> GetByOrderIdAsync(int orderId);
}
