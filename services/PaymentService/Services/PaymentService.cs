using PaymentService.DTOs;
using PaymentService.Mappings;
using PaymentService.Messaging;
using PaymentService.Models;
using PaymentService.Providers;
using PaymentService.Repositories;

namespace PaymentService.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repository;
    private readonly IPaymentEventPublisher _publisher;
    private readonly IPaymentProvider _provider;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        IPaymentRepository repository,
        IPaymentEventPublisher publisher,
        IPaymentProvider provider,
        ILogger<PaymentService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _provider = provider;
        _logger = logger;
    }

    public async Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentRequestDto request)
    {
        var payment = request.ToEntity();

        var (success, resultCode) = _provider.Process(request.Amount);

        payment.Status = success
            ? PaymentStatus.Completed.ToString().ToUpper()
            : PaymentStatus.Failed.ToString().ToUpper();
        payment.ResultCode = resultCode; 

        await _repository.AddAsync(payment);
        await _repository.SaveChangesAsync();

        if (success)
        {
            await _publisher.PublishPaymentCompletedAsync(payment);
            _logger.LogInformation("[Payment] Completed for OrderId={OrderId} Amount={Amount}",
                payment.OrderId, payment.Amount);
        }
        else
        {
            await _publisher.PublishPaymentFailedAsync(payment);
            _logger.LogWarning("[Payment] CONTROLLED_TEST_FAILURE for OrderId={OrderId} Amount={Amount}",
                payment.OrderId, payment.Amount);
        }

        return payment.ToDto();
    }
}