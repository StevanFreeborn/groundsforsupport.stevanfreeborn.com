using System.Text.Json;

using GroundsForSupport.Server.Data;
using GroundsForSupport.Server.Payments.Endpoints;
using GroundsForSupport.Server.Payments.Stripe;
using GroundsForSupport.Server.RateLimiting;

using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddValidation();

builder.Services.AddProblemDetails();

builder.Services.AddHttpClient();

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.Configure<ForwardedHeadersOptions>(
  static options => options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
);

builder.Services.AddRateLimingPolicies();

builder.Services.ConfigureOptions<ContextOptionsSetup>();
builder.Services.AddDbContext<Context>();
builder.Services.AddHostedService<MigrationService>();

builder.Services.ConfigureOptions<StripeOptionsSetup>();
builder.Services.AddSingleton<IStripeService, StripeService>();

builder.Services.ConfigureHttpJsonOptions(
  static options => options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

if (app.Environment.IsProduction())
{
  app.UseForwardedHeaders();
  app.UseRateLimiter();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseStatusCodePages();

app.UseHttpsRedirection();
app.UseHsts();

app.MapCreatePaymentIntentEndpoint();
app.MapGetPaymentsEndpoint();
app.MapEventsEndpoint();

app.Run();