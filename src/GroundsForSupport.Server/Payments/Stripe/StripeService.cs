using GroundsForSupport.Server.Data;

using Microsoft.Extensions.Options;

using Stripe;

namespace GroundsForSupport.Server.Payments.Stripe;

internal sealed class StripeService(
  IOptions<StripeOptions> options,
  HttpClient httpClient,
  ILogger<StripeService> logger
) : IStripeService
{
  private readonly StripeClient _client = new(options.Value.ApiKey, httpClient: new SystemNetHttpClient(httpClient));
  private readonly ILogger<StripeService> _logger = logger;

  public async Task<(bool IsSuccess, Intent Intent)> CreatePaymentIntentAsync(string name, decimal amount, string? message, string? email)
  {
    try
    {
      var createOptions = new PaymentIntentCreateOptions
      {
        Amount = (long)(amount * 100),
        Currency = "usd",
        AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
        {
          Enabled = true,
        },
        Metadata = new Dictionary<string, string>
        {
          { nameof(Payment.Name), name },
          { nameof(Payment.Message), message ?? string.Empty },
        },
      };

      if (string.IsNullOrWhiteSpace(email) is false)
      {
        createOptions.ReceiptEmail = email;
      }

      var intent = await _client.V1.PaymentIntents.CreateAsync(createOptions);

      return (true, new Intent(intent.ClientSecret));
    }
    catch (Exception)
    {
      _logger.LogError("Failed to create Stripe payment intent for {Name} with amount {Amount}", name, amount);
      return (false, new Intent(string.Empty));
    }
  }
}