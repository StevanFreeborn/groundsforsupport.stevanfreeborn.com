namespace GroundsForSupport.Server.Payments.Stripe;

internal interface IStripeService
{
  Task<(bool IsSuccess, Intent Intent)> CreatePaymentIntentAsync(
    string name,
    decimal amount,
    string? message,
    string? email,
    CancellationToken cancellationToken = default
  );
}