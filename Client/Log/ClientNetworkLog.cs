using Microsoft.Extensions.Logging;

namespace Client.Log;

public static partial class ClientNetworkLog
{
    [LoggerMessage(LogLevel.Information, "Started listening. Receive buffer size: {bufferSize}")]
    public static partial void StartListen(ILogger logger, int bufferSize);
}