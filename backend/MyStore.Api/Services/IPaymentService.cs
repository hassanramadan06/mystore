namespace MyStore.Api.Services;

/// <summary>
/// Abstraction over a payment provider (Stripe). The default implementation is a stub
/// that returns fake intent ids — replace with a real Stripe-backed implementation when keys
/// are configured.
/// </summary>
public interface IPaymentService
{
    Task<PaymentIntent> CreateIntentAsync(decimal amount, string currency, int orderId, int userId);
    string PublishableKey { get; }
}

public record PaymentIntent(string Id, string ClientSecret);
