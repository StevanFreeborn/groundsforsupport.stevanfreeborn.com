namespace GroundsForSupport.Server.TextToSpeech;

internal interface IStreamerBotService
{
  Task TriggerTextToSpeechAsync(
    string name,
    long amount,
    string message,
    CancellationToken cancellationToken = default
  );
}