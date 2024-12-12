using Microsoft.Extensions.Logging;

namespace Server;

public static partial class ServerLog
{
    [LoggerMessage(LogLevel.Information, "Application started. Port: {port}")]
    public static partial void Start(ILogger logger, int port);
    
    [LoggerMessage(LogLevel.Information, "New user connected. Address: {address}")]
    public static partial void UserConnected(ILogger logger, string address);
    
    [LoggerMessage(LogLevel.Warning, "Unknown command was received: {commandName}")]
    public static partial void UnknownCommand(ILogger logger, string commandName);
    
    [LoggerMessage(LogLevel.Warning, "Trying accessing the socket. Client was disconnected: {address}, Number of current connected clients: {numberOfClients}")]
    public static partial void ClientIsNotAccessable(ILogger logger, string address, int numberOfClients);

    [LoggerMessage(LogLevel.Warning, "The unexpected error occured: {message} {stackTrace}")]
    public static partial void RuntimeWarning(ILogger logger, string message, string? stackTrace);

    [LoggerMessage(LogLevel.Critical, "Application was stopped. Cannot parse port: {port}")]
    public static partial void StartupCriticalError(ILogger logger, string port);
}