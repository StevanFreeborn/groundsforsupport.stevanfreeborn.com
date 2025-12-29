using System.Diagnostics;
using System.Diagnostics.Metrics;
using GroundsForSupport.Server;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Register custom metrics
builder.Services.AddSingleton<ApplicationMetrics>();

// Configure OpenTelemetry
var serviceName = "GroundsForSupport";
var serviceVersion = "1.0.0";

builder.Services.AddOpenTelemetry()
  .ConfigureResource(resource => resource
    .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
  .WithTracing(tracing => tracing
    .AddAspNetCoreInstrumentation()
    .AddHttpClientInstrumentation()
    .AddConsoleExporter())
  .WithMetrics(metrics => metrics
    .AddAspNetCoreInstrumentation()
    .AddHttpClientInstrumentation()
    .AddMeter("GroundsForSupport.Payments")
    .AddConsoleExporter());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

// Example endpoint demonstrating custom metrics
app.MapPost("/api/test-payment", (ApplicationMetrics metrics) =>
{
  metrics.RecordPaymentAttempt();

  // Simulate a payment attempt
  var random = new Random();
  var isSuccess = random.Next(0, 2) == 1;
  var amount = random.NextDouble() * 100;

  if (isSuccess)
  {
    metrics.RecordPaymentSuccess(amount);
    return Results.Ok(new { success = true, amount });
  }

  metrics.RecordPaymentFailure();
  return Results.BadRequest(new { success = false });
});

app.Run();