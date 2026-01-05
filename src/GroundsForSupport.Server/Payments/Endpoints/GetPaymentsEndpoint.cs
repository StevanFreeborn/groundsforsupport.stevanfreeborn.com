using System.ComponentModel.DataAnnotations;
using System.Globalization;

using GroundsForSupport.Server.Data;

using Microsoft.AspNetCore.Mvc;

namespace GroundsForSupport.Server.Payments.Endpoints;

internal static class GetPaymentsEndpoint
{
  private const string Route = "/payments";

  public static IEndpointConventionBuilder MapGetPaymentsEndpoint(this WebApplication app)
  {
    return app.MapGet(Route, GetPaymentsHandler);
  }

  public static async Task<IResult> GetPaymentsHandler(
    [AsParameters] Request request,
    [FromServices] Context context
  )
  {
    var validationErrors = request.Validate(new ValidationContext(request));

    if (validationErrors.Any())
    {
      return Results.ValidationProblem(validationErrors
        .GroupBy(static e => e.MemberNames.FirstOrDefault() ?? string.Empty)
        .ToDictionary(static g => g.Key, static g => g.Select(static e => e.ErrorMessage ?? string.Empty).ToArray()));
    }

    var totalNumberOfPayments = context.Payments.Count();
    var totalNumberOfPages = (int)Math.Ceiling(totalNumberOfPayments / (double)request.PageSize);

    var paymentsQuery = request.SortDirection?.ToLower(CultureInfo.CurrentCulture) is "asc"
      ? context.Payments
        .OrderBy(static p => p.CreatedAtUnix)
      : context.Payments
        .OrderByDescending(static p => p.CreatedAtUnix);

    var payments = paymentsQuery
      .Skip((request.PageNumber - 1) * request.PageSize)
      .Take(request.PageSize)
      .Select(PaymentRecord.From)
      .ToList();

    var response = new Response(
      totalNumberOfPayments,
      totalNumberOfPages,
      request.PageNumber,
      payments
    );

    return Results.Ok(response);
  }

  internal sealed record Request(int PageSize = 50, int PageNumber = 1, string? SortDirection = null) : IValidatableObject
  {
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      if (PageSize <= 0)
      {
        yield return new ValidationResult("PageSize must be greater than zero", [nameof(PageSize)]);
      }

      if (PageNumber <= 0)
      {
        yield return new ValidationResult("PageNumber must be greater than zero", [nameof(PageNumber)]);
      }

      if (
        SortDirection is not null &&
        SortDirection.ToLower(CultureInfo.CurrentCulture) is not "asc" and not "desc"
      )
      {
        yield return new ValidationResult("SortDirection must be either 'asc' or 'desc'", [nameof(SortDirection)]);
      }
    }
  }

  internal sealed record Response(
    int TotalNumberOfPayments,
    int TotalNumberOfPages,
    int CurrentPageNumber,
    List<PaymentRecord> Payments
  );

  internal sealed record PaymentRecord
  {
    public string Name { get; init; } = string.Empty;
    public long Amount { get; init; }
    public string? Message { get; init; }
    public long CreatedAtUnix { get; init; }

    public static PaymentRecord From(Payment payment)
    {
      return new PaymentRecord
      {
        Amount = payment.Amount,
        Name = payment.Name,
        Message = payment.Message,
        CreatedAtUnix = payment.CreatedAtUnix,
      };
    }
  }
}