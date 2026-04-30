namespace MyStore.Api.Services;

/// <summary>
/// Stub Stripe payment service. Returns deterministic fake intent ids so the frontend can
/// complete a checkout flow without real Stripe credentials. Swap with a Stripe-backed
/// implementation once keys are configured.
/// </summary>
public class StubPaymentService : IPaymentService
{
    private readonly IConfiguration _config;

    public StubPaymentService(IConfiguration config) => _config = config;

    public string PublishableKey => _config["Stripe:PublishableKey"] ?? "pk_test_stub";

    public Task<PaymentIntent> CreateIntentAsync(decimal amount, string currency, int orderId, int userId)
    {
        var id = $"pi_stub_{orderId}_{Guid.NewGuid():N}";
        var secret = $"{id}_secret_stub";
        return Task.FromResult(new PaymentIntent(id, secret));
    }
}
