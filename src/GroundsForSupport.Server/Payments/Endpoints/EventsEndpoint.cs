using GroundsForSupport.Server.Data;
using GroundsForSupport.Server.Payments.Stripe;
using GroundsForSupport.Server.TextToSpeech;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Stripe;

namespace GroundsForSupport.Server.Payments.Endpoints;

internal static class EventsEndpoint
{
  private const string Route = "/payments/events";
  private const string StripeSignatureHeader = "Stripe-Signature";

  internal static IEndpointConventionBuilder MapEventsEndpoint(this WebApplication app)
  {
    return app.MapPost(Route, EventsHandler);
  }

  internal static async Task<IResult> EventsHandler(
    HttpContext httpContext,
    [FromServices] IOptions<StripeOptions> options,
    [FromServices] Context dbContext,
    [FromServices] TimeProvider timeProvider,
    [FromServices] IStreamerBotService streamerBotService,
    [FromServices] ILogger<Program> logger,
    CancellationToken ct
  )
  {
    var json = await new StreamReader(httpContext.Request.Body).ReadToEndAsync(ct);

    try
    {
      var signatureHeader = httpContext.Request.Headers[StripeSignatureHeader];
      var stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, options.Value.EventsWebhookSecret);

      if (stripeEvent.Type is not EventTypes.PaymentIntentSucceeded)
      {
        return Results.Ok();
      }

      var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;

      var name = paymentIntent.Metadata.TryGetValue(nameof(Payment.Name), out var metaName)
        ? metaName
        : "unknown";

      var message = paymentIntent.Metadata.TryGetValue(nameof(Payment.Message), out var metaMessage)
        ? metaMessage
        : string.Empty;

      var existingPayment = await dbContext.Payments
        .FirstOrDefaultAsync(p => p.Id == paymentIntent.Id, ct);

      if (existingPayment is not null)
      {
        logger.LogWarning("Payment with ID {PaymentId} already exists. Skipping.", paymentIntent.Id);
        return Results.Ok();
      }

      var payment = new Payment()
      {
        Id = paymentIntent.Id,
        Amount = paymentIntent.AmountReceived,
        Name = name,
        Message = message,
        CreatedAtUnix = timeProvider.GetUtcNow().ToUnixTimeMilliseconds(),
      };

      await dbContext.Payments.AddAsync(payment, ct);
      await dbContext.SaveChangesAsync(ct);

      try
      {
        await streamerBotService.TriggerTextToSpeechAsync(
          name,
          paymentIntent.AmountReceived,
          message,
          ct
        );
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Failed to trigger text-to-speech for payment {PaymentId}", paymentIntent.Id);
      }

      return Results.Ok();
    }
    catch (StripeException e)
    {
      Console.WriteLine($"Stripe exception: {e.Message}");
      return Results.BadRequest(new { error = e.Message });
    }
  }
}