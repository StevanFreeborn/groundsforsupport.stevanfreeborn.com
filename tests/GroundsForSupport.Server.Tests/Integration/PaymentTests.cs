using System.Net;
using System.Net.Http.Json;
using GroundsForSupport.API.Tests.Integration.Infra;

namespace GroundsForSupport.API.Tests.Integration;

public sealed class PaymentTests(TestApi api) : IClassFixture<TestApi>
{
  private readonly TestApi _api = api;

  [Fact]
  public async Task CreatePaymentIntent_WhenGivenInvalidAmount_ItShouldReturnBadRequest()
  {
    var client = _api.CreateClient();

    var request = new {
      amount = 0,
      email = string.Empty,
    };

    var response = await client.PostAsJsonAsync("/create-payment-intent", request, TestContext.Current.CancellationToken);

    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }
}