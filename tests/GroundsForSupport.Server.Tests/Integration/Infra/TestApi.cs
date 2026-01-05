using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using GroundsForSupport.Server.Data;
using GroundsForSupport.Server.Payments.Stripe;

using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace GroundsForSupport.Server.Tests.Integration.Infra;

public sealed class TestApi : WebApplicationFactory<Program>
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    var contextOptions = new ContextOptions()
    {
      DatabaseFilePath = "GroundsForSupport.Test.db",
    };

    var stripeOptions = new StripeOptions()
    {
      ApiKey = "sk_test_12345",
    };

    var contextOptionsJson = JsonSerializer.Serialize(contextOptions);
    var stripeOptionsJson = JsonSerializer.Serialize(stripeOptions);
    
    var configJson = $@"{{
      ""{nameof(ContextOptions)}"": {contextOptionsJson},
      ""{nameof(StripeOptions)}"": {stripeOptionsJson}
    }}";

    var config = new ConfigurationBuilder()
      .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(configJson)))
      .Build();

    builder.UseConfiguration(config);
  }
}