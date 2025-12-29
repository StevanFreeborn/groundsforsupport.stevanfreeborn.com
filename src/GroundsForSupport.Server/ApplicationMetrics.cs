using System.Diagnostics.Metrics;

namespace GroundsForSupport.Server;

public class ApplicationMetrics
{
  private readonly Meter _meter;
  private readonly Counter<long> _paymentAttempts;
  private readonly Counter<long> _paymentSuccesses;
  private readonly Counter<long> _paymentFailures;
  private readonly Histogram<double> _paymentAmount;

  public ApplicationMetrics(IMeterFactory meterFactory)
  {
    _meter = meterFactory.Create("GroundsForSupport.Payments");

    _paymentAttempts = _meter.CreateCounter<long>(
      "payment.attempts",
      description: "Total number of payment attempts");

    _paymentSuccesses = _meter.CreateCounter<long>(
      "payment.successes",
      description: "Total number of successful payments");

    _paymentFailures = _meter.CreateCounter<long>(
      "payment.failures",
      description: "Total number of failed payments");

    _paymentAmount = _meter.CreateHistogram<double>(
      "payment.amount",
      unit: "USD",
      description: "Distribution of payment amounts");
  }

  public void RecordPaymentAttempt() => _paymentAttempts.Add(1);

  public void RecordPaymentSuccess(double amount)
  {
    _paymentSuccesses.Add(1);
    _paymentAmount.Record(amount);
  }

  public void RecordPaymentFailure() => _paymentFailures.Add(1);
}
