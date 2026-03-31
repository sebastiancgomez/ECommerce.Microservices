using PaymentService.DTOs;
using PaymentService.Messaging;
using PaymentService.Models;
using PaymentService.Repositories;
using PaymentService.Services;

namespace PaymentService.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repository;
    private readonly IPaymentEventPublisher _publisher;

    public PaymentService(
        IPaymentRepository repository,
        IPaymentEventPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<Payment> CreatePaymentAsync(CreatePaymentRequest request)
    {
        var payment = new Payment
        {
            OrderId = request.OrderId,
            Amount = request.Amount,
            Status = "Completed" // simplificado por ahora
        };

        var created = await _repository.AddAsync(payment);

        await _publisher.PublishPaymentCompletedAsync(created);

        return created;
    }
}