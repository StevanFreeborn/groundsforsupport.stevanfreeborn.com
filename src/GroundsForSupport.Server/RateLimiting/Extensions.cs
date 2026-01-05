using System.Globalization;
using System.Threading.RateLimiting;

namespace GroundsForSupport.Server.RateLimiting;

internal static class Extensions
{
  public static void AddRateLimingPolicies(this IServiceCollection services)
  {
    services.AddRateLimiter(static o =>
    {
      o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

      o.OnRejected = static (context, ct) =>
      {
        var response = context.HttpContext.Response;

        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
          response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString(CultureInfo.InvariantCulture);
          var resetTime = DateTimeOffset.UtcNow.Add(retryAfter);
          var unixTimeMilliseconds = resetTime.ToUnixTimeMilliseconds();
          response.Headers.Append("X-RateLimit-Reset", unixTimeMilliseconds.ToString(CultureInfo.InvariantCulture));
        }

        return ValueTask.CompletedTask;
      };

      o.AddPolicy(FixedRateLimitPolicy.Name, FixedRateLimitPolicy.Partitioner);
    });
  }
}