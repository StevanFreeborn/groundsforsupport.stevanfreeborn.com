namespace GroundsForSupport.Server.Payments;

internal sealed class Payment
{
  public required string Id { get; init; }
  public required string Name { get; init; }
  public required long Amount { get; init; }
  public string? Message { get; init; }
  public long CreatedAtUnix { get; init; }
}