using PaymentService.DTOs;
using PaymentService.Mappings;
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

    public async Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentRequestDto request)
    {
        var payment = request.ToEntity();

        
        // 🔥 Simulación de pago
        if (request.Amount < 1000)
        {
            payment.Status = PaymentStatus.Completed.ToString().ToUpper();
        }
        else
        {
            payment.Status = PaymentStatus.Failed.ToString().ToUpper();
        }
        await _repository.AddAsync(payment);
        await _repository.SaveChangesAsync();

        return payment.ToDto();
    }
}