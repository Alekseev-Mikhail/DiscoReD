using Microsoft.Extensions.Logging;

namespace Client.Log;

public static partial class VoiceClientLog
{
    [LoggerMessage(LogLevel.Information, "Initialization was started. The host address is {address} and port is {port}.")]
    public static partial void StartInit(ILogger logger, string address, int port);
    
    [LoggerMessage(LogLevel.Information, "Initialization of {initOf} was started.")]
    public static partial void InitOf(ILogger logger, string initOf);
    
    [LoggerMessage(LogLevel.Information, "Initialization was finished!")]
    public static partial void FinishInit(ILogger logger);
}