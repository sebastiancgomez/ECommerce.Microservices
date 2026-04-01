namespace OrderService.Clients;
using OrderService.DTOs;

public interface IPaymentClient
{
    Task<PaymentResponseDto> CreatePayment(CreatePaymentRequestDto request);
}
