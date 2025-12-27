using System.ComponentModel.DataAnnotations;
using System.Text.Json;

using Microsoft.Extensions.Options;

using Stripe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddValidation();

builder.Services.AddProblemDetails();

builder.Services.AddHttpClient();

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

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseStatusCodePages();

app.UseHttpsRedirection();

app.MapPost("/create-payment-intent", CreatePaymentIntentHandler);

app.Run();

static async Task<IResult> CreatePaymentIntentHandler(
  CreatePaymentIntentRequest request,
  IStripeService stripeService
)
{
  var validationErrors = request.Validate(new ValidationContext(request));

  if (validationErrors.Any())
  {
    return Results.ValidationProblem(validationErrors
      .GroupBy(static e => e.MemberNames.FirstOrDefault() ?? string.Empty)
      .ToDictionary(static g => g.Key, static g => g.Select(static e => e.ErrorMessage ?? string.Empty).ToArray()));
  }

  var (isSuccess, intent) = await stripeService.CreatePaymentIntentAsync(request.Amount, request.Email);

  if (isSuccess is false)
  {
    return Results.InternalServerError();
  }

  return Results.Ok(intent);
}

internal sealed record CreatePaymentIntentRequest(decimal Amount, string? Email)
{
  public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
  {
    if (Amount <= 0)
    {
      yield return new ValidationResult("Amount must be greater than zero", [nameof(Amount)]);
    }

    if (string.IsNullOrWhiteSpace(Email) is false && new EmailAddressAttribute().IsValid(Email) is false)
    {
      yield return new ValidationResult("Email is not valid", [nameof(Email)]);
    }
  }
}

internal sealed record StripeOptions
{
  public string ApiKey { get; init; } = string.Empty;
}

internal sealed record StripeOptionsSetup : IConfigureOptions<StripeOptions>
{
  private const string SectionName = nameof(StripeOptions);
  private readonly IConfiguration _configuration;

  public StripeOptionsSetup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public void Configure(StripeOptions options)
  {
    _configuration.GetSection(SectionName).Bind(options);
  }
}

internal sealed record Intent(string ClientSecret);

internal interface IStripeService
{
  Task<(bool IsSuccess, Intent Intent)> CreatePaymentIntentAsync(decimal amount, string? email);
}

internal sealed class StripeService(
  IOptions<StripeOptions> options,
  HttpClient httpClient
) : IStripeService
{
  private readonly StripeClient _client = new(options.Value.ApiKey, httpClient: new SystemNetHttpClient(httpClient));

  public async Task<(bool IsSuccess, Intent Intent)> CreatePaymentIntentAsync(decimal amount, string? email)
  {
    try
    {
      var createOptions = new PaymentIntentCreateOptions
      {
        Amount = (long)(amount * 100),
        Currency = "usd",
        AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
        {
          Enabled = true,
        },
      };

      if (string.IsNullOrWhiteSpace(email) is false)
      {
        createOptions.ReceiptEmail = email;
      }

      var intent = await _client.V1.PaymentIntents.CreateAsync(createOptions);

      return (true, new Intent(intent.ClientSecret));
    }
    catch (Exception)
    {
      // TODO: Log exception
      return (false, new Intent(string.Empty));
    }
  }
}