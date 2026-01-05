using System.Threading.RateLimiting;

namespace GroundsForSupport.Server.RateLimiting;

internal static class FixedRateLimitPolicy
{
  public const string Name = "fixed";

  public static Func<HttpContext, RateLimitPartition<string>> Partitioner =>
    static context => RateLimitPartition.GetFixedWindowLimiter(
      partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
      factory: static partition => new FixedWindowRateLimiterOptions
      {
        PermitLimit = 100,
        Window = TimeSpan.FromHours(1),
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        QueueLimit = 0
      }
    );
}