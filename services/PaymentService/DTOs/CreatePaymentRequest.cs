
namespace PaymentService.DTOs;

public class CreatePaymentRequest
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
}

