using Microsoft.Extensions.Options;

namespace GroundsForSupport.Server.Payments.Stripe;

internal sealed record StripeOptions
{
  public string ApiKey { get; init; } = string.Empty;
  public string EventsWebhookSecret { get; init; } = string.Empty;
}

internal sealed record StripeOptionsSetup : IConfigureOptions<StripeOptions>
{
  private const string SectionName = nameof(StripeOptions);
  private readonly IConfiguration _configuration;

  public StripeOptionsSetup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public void Configure(StripeOptions options)
  {
    _configuration.GetSection(SectionName).Bind(options);
  }
}