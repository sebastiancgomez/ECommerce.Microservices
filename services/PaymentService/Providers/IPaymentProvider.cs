namespace PaymentService.Providers;

public interface IPaymentProvider
{
    (bool Success, string ResultCode) Process(decimal amount);
}