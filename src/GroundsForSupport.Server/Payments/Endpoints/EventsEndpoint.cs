using GroundsForSupport.Server.Data;
using GroundsForSupport.Server.Payments.Stripe;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Stripe;

namespace GroundsForSupport.Server.Payments.Endpoints;

internal static class EventsEndpoint
{
  private const string Route = "/events";
  private const string StripeSignatureHeader = "Stripe-Signature";

  internal static IEndpointConventionBuilder MapEventsEndpoint(this WebApplication app)
  {
    return app.MapPost(Route, EventsHandler);
  }

  internal static async Task<IResult> EventsHandler(
    HttpContext httpContext,
    [FromServices] IOptions<StripeOptions> options,
    [FromServices] Context dbContext,
    [FromServices] TimeProvider timeProvider
  )
  {
    var json = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();

    try
    {
      var stripeEvent = EventUtility.ParseEvent(json);
      var signatureHeader = httpContext.Request.Headers[StripeSignatureHeader];
      stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, options.Value.EventsWebhookSecret);

      if (stripeEvent.Type is not EventTypes.PaymentIntentSucceeded)
      {
        return Results.Ok();
      }

      var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
      var name = paymentIntent.Metadata[nameof(Payment.Name)];
      var message = paymentIntent.Metadata[nameof(Payment.Message)];

      var payment = new Payment()
      {
        Id = paymentIntent.Id,
        Amount = paymentIntent.AmountReceived,
        Name = name,
        Message = message,
        CreatedAtUnix = timeProvider.GetUtcNow().ToUnixTimeMilliseconds(),
      };

      dbContext.Payments.Add(payment);
      await dbContext.SaveChangesAsync();

      return Results.Ok();
    }
    catch (StripeException e)
    {
      Console.WriteLine($"Stripe exception: {e.Message}");
      return Results.BadRequest(new { error = e.Message });
    }
  }
}