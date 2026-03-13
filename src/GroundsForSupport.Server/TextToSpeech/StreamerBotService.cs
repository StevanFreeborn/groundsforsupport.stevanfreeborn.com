using System.Text.Json;

using Microsoft.Extensions.Options;

namespace GroundsForSupport.Server.TextToSpeech;

internal sealed class StreamerBotService(
  IHttpClientFactory httpClientFactory,
  IOptionsMonitor<StreamerBotOptions> options
) : IStreamerBotService
{
  private const string DoActionEndpoint = "/DoAction";
  private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
  private readonly IOptionsMonitor<StreamerBotOptions> _options = options;

  public Task TriggerTextToSpeechAsync(string name, long amount, string message, CancellationToken cancellationToken = default)
  {
    var client = _httpClientFactory.CreateClient();
    var url = new UriBuilder(_options.CurrentValue.GetBaseUrl())
    {
      Path = DoActionEndpoint
    }.Uri;

    var request = new HttpRequestMessage(HttpMethod.Post, url);

    var body = new
    {
      action = new
      {
        id = _options.CurrentValue.TextToSpeechActionId
      },
      args = new
      {
        name,
        amount = $"${amount / 100.0:F2}",
        message,
        apiKey = _options.CurrentValue.ApiKey
      }
    };

    var json = JsonSerializer.Serialize(body);
    request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

    return client.SendAsync(request, cancellationToken);
  }
}