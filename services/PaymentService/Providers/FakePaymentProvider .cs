namespace PaymentService.Providers;

public class FakePaymentProvider : IPaymentProvider
{
    private readonly ILogger<FakePaymentProvider> _logger;
    private static readonly Random _random = new();

    private const int SuccessThreshold = 85; // 85% éxito, 15% fallo

    public FakePaymentProvider(ILogger<FakePaymentProvider> logger)
    {
        _logger = logger;
    }

    public (bool Success, string ResultCode) Process(decimal amount)
    {
        var roll = _random.Next(1, 101);
        var success = roll <= SuccessThreshold;

        _logger.LogInformation(
            "[FakePaymentProvider] Roll={Roll} Threshold={Threshold} Result={Result} Amount={Amount}",
            roll, SuccessThreshold, success ? "SUCCESS" : "CONTROLLED_TEST_FAILURE", amount);

        return success
            ? (true, "SUCCESS")
            : (false, "CONTROLLED_TEST_FAILURE");
    }
}
