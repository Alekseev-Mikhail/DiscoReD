using Microsoft.Extensions.Logging;

namespace Client.Log;

public static partial class VoiceInputLog
{
    [LoggerMessage(LogLevel.Information, "Started listen your beautiful voice")]
    public static partial void Start(ILogger logger);
}