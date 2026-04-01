
using PaymentService.Models;

namespace PaymentService.DTOs;

public class CreatePaymentRequestDto
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = "FAKE";
    public string Currency { get; set; } = CurrencyType.USD.ToString();
}

