using System.ComponentModel.DataAnnotations;

using GroundsForSupport.Server.Payments.Stripe;
using GroundsForSupport.Server.RateLimiting;

namespace GroundsForSupport.Server.Payments.Endpoints;

internal static class CreatePaymentIntentEndpoint
{
  private const string Route = "/payments/create-intent";

  public static IEndpointConventionBuilder MapCreatePaymentIntentEndpoint(this WebApplication app)
  {
    return app.MapPost(Route, CreatePaymentIntentHandler).RequireRateLimiting(FixedRateLimitPolicy.Name);
  }

  public static async Task<IResult> CreatePaymentIntentHandler(
    Request request,
    IStripeService stripeService,
    CancellationToken cancellationToken
  )
  {
    var validationErrors = request.Validate(new ValidationContext(request));

    if (validationErrors.Any())
    {
      return Results.ValidationProblem(validationErrors
        .GroupBy(static e => e.MemberNames.FirstOrDefault() ?? string.Empty)
        .ToDictionary(static g => g.Key, static g => g.Select(static e => e.ErrorMessage ?? string.Empty).ToArray()));
    }

    var (isSuccess, intent) = await stripeService.CreatePaymentIntentAsync(
      request.Name,
      request.Amount,
      request.Message,
      request.Email,
      cancellationToken
    );

    if (isSuccess is false)
    {
      return Results.InternalServerError();
    }

    return Results.Ok(intent);
  }

  internal sealed record Request(
    string Name,
    decimal Amount,
    string? Message,
    string? Email
  )
  {
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      if (string.IsNullOrWhiteSpace(Name))
      {
        yield return new ValidationResult("Name is required", [nameof(Name)]);
      }

      if (Amount <= 0)
      {
        yield return new ValidationResult("Amount must be greater than zero", [nameof(Amount)]);
      }

      if (Message?.Length > 250)
      {
        yield return new ValidationResult("Message cannot exceed 250 characters", [nameof(Message)]);
      }

      if (string.IsNullOrWhiteSpace(Email) is false && new EmailAddressAttribute().IsValid(Email) is false)
      {
        yield return new ValidationResult("Email is not valid", [nameof(Email)]);
      }
    }
  }

  internal sealed record Response(string ClientSecret);
}