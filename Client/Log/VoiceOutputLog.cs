using Microsoft.Extensions.Logging;

namespace Client.Log;

public static partial class VoiceOutputLog
{
    [LoggerMessage(LogLevel.Information, "Started playing")]
    public static partial void StartPlay(ILogger logger);
}