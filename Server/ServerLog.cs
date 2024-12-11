using Microsoft.Extensions.Logging;

namespace Server;

public static partial class ServerLog
{
    [LoggerMessage(LogLevel.Information, "Application started. Port: {port}")]
    public static partial void Startup(ILogger logger, int port);
    
    [LoggerMessage(LogLevel.Information, "New user connected. Address: {address}")]
    public static partial void UserConnected(ILogger logger, string address);
    
    [LoggerMessage(LogLevel.Warning, "Command has a illegal size: {size}")]
    public static partial void IllegalCommandSize(ILogger logger, int size);
    
    [LoggerMessage(LogLevel.Warning, "Unknown command was received: {commandName}")]
    public static partial void UnknownCommand(ILogger logger, int commandName);
    
    [LoggerMessage(LogLevel.Warning, "Trying accessing the socket. Client was disconnected: {address}")]
    public static partial void ClientWasDisconnectedWhileAccessing(ILogger logger, string address);
    
    [LoggerMessage(LogLevel.Critical, "Application was stopped. Cannot parse port: {port}")]
    public static partial void StartupCriticalError(ILogger logger, string port);
    
    [LoggerMessage(LogLevel.Critical, "The unexpected error occured: {message}")]
    public static partial void RuntimeCriticalError(ILogger logger, string? message);
}