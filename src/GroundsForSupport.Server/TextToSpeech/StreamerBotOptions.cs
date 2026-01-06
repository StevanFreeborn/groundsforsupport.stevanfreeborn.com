using Microsoft.Extensions.Options;

namespace GroundsForSupport.Server.TextToSpeech;

internal sealed record StreamerBotOptions
{
  public string Host { get; init; } = string.Empty;
  public int Port { get; init; }
  public string ApiKey { get; init; } = string.Empty;
  public string TextToSpeechActionId { get; init; } = string.Empty;

  public string GetBaseUrl()
  {
    return $"http://{Host}:{Port}";
  }
}

internal sealed record StreamerBotOptionsSetup : IConfigureOptions<StreamerBotOptions>
{
  private const string SectionName = nameof(StreamerBotOptions);
  private readonly IConfiguration _configuration;

  public StreamerBotOptionsSetup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public void Configure(StreamerBotOptions options)
  {
    _configuration.GetSection(SectionName).Bind(options);
  }
}