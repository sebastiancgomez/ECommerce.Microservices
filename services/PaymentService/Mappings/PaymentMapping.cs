using PaymentService.DTOs;
using PaymentService.Models;

namespace PaymentService.Mappings;

public static class PaymentMapping
{
    public static Payment ToEntity(this CreatePaymentRequestDto request)
    {
        return new Payment
        {
            OrderId = request.OrderId,
            Amount = request.Amount,
            Currency = request.Currency.ToUpper(),
            Method = request.Method.ToUpper(),
            Status = PaymentStatus.Pending.ToString(),
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public static PaymentResponseDto ToDto(this Payment payment)
    {
        return new PaymentResponseDto
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            Currency = payment.Currency,
            Status = payment.Status,
            Method = payment.Method,
            CreatedAt = payment.CreatedAt
        };
    }
}