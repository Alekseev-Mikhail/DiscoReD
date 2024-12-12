using Microsoft.Extensions.Logging;

namespace Client.Log;

public static partial class ClientNetworkLog
{
    [LoggerMessage(LogLevel.Information, "Started listening. Receive buffer size: {bufferSize}")]
    public static partial void StartListen(ILogger logger, int bufferSize);

    [LoggerMessage(LogLevel.Information, "Sending bodiless command: {name}")]
    public static partial void SendingBodilessCommand(ILogger logger, string name);

    [LoggerMessage(LogLevel.Information, "Received response on bodiless command: {response}")]
    public static partial void BodilessCommandResponse(ILogger logger, int response);

    [LoggerMessage(LogLevel.Information, "Pong!")]
    public static partial void Pong(ILogger logger);
    
    [LoggerMessage(LogLevel.Warning, "The unexpected error occured: {message} {stackTrace}")]
    public static partial void RuntimeWarning(ILogger logger, string message, string? stackTrace);
}