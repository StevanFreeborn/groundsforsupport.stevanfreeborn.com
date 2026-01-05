using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GroundsForSupport.Server.Logging;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
  private readonly ILogger<GlobalExceptionHandler> _logger = logger;

  public async ValueTask<bool> TryHandleAsync(
    HttpContext httpContext,
    Exception exception,
    CancellationToken cancellationToken
  )
  {
    _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

    var problemDetails = new ProblemDetails
    {
      Status = StatusCodes.Status500InternalServerError,
      Title = "Server Error",
      Detail = "An unexpected error occurred. Please try again later."
    };

    httpContext.Response.StatusCode = problemDetails.Status.Value;

    await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

    return true;
  }
}