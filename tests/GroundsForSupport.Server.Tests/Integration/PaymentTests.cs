using System.Net;
using System.Net.Http.Json;

using GroundsForSupport.Server.Payments;
using GroundsForSupport.Server.Payments.Stripe;
using GroundsForSupport.Server.Tests.Integration.Infra;

using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using Moq;

[assembly: CaptureConsole]

namespace GroundsForSupport.API.Tests.Integration;

public sealed class PaymentTests(TestApi api) : IClassFixture<TestApi>
{
  private readonly TestApi _api = api;

  [Fact]
  public async Task CreatePaymentIntent_WhenGivenInvalidAmount_ItShouldReturnBadRequest()
  {
    var client = _api.CreateClient();

    var request = new
    {
      amount = 0,
      email = string.Empty,
    };

    var response = await client.PostAsJsonAsync("/payments/create-intent", request, TestContext.Current.CancellationToken);

    Console.WriteLine(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Fact]
  public async Task CreatePaymentIntent_WhenGivenInvalidEmail_ItShouldReturnBadRequest()
  {
    var client = _api.CreateClient();

    var request = new
    {
      amount = 5000,
      email = "invalid-email",
    };

    var response = await client.PostAsJsonAsync("/payments/create-intent", request, TestContext.Current.CancellationToken);

    Console.WriteLine(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Fact]
  public async Task CreatePaymentIntent_WhenGivenValidRequest_ItShouldReturnCreated()
  {
    var mockStripeService = new Mock<IStripeService>();

    mockStripeService
      .Setup(s => s.CreatePaymentIntentAsync(
          It.IsAny<string>(),
          It.IsAny<decimal>(),
          It.IsAny<string?>(),
          It.IsAny<string?>(),
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync((true, new Intent("pi_12345_secret_67890")));

    var api = _api.WithWebHostBuilder(
      b => b.ConfigureTestServices(
        s => s.AddSingleton(mockStripeService.Object)
      )
    );

    var client = api.CreateClient();

    var request = new
    {
      name = "Test User",
      amount = 5000,
      email = "test@test.com",
    };

    var response = await client.PostAsJsonAsync("/payments/create-intent", request, TestContext.Current.CancellationToken);

    Console.WriteLine(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }
}