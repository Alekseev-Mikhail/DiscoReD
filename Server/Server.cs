using System.Net;
using System.Net.Sockets;
using System.Text;
using Core;
using Microsoft.Extensions.Logging;

namespace Server;

public class Guild
{
    private readonly int port;
    private readonly UdpClient listener;
    private readonly List<IPEndPoint> connectedUsers = [];
    private readonly ILogger logger;
    private IPEndPoint lastEndpoint;

    public Guild(ILoggerFactory factory, string portAsString)
    {
        logger = factory.CreateLogger<Guild>();
        
        try
        {
            port = int.Parse(portAsString);
        }
        catch (Exception)
        {
            AppLog.StartupCriticalError(logger, portAsString);
            throw;
        }
        
        listener = new UdpClient(port);
        lastEndpoint = new IPEndPoint(IPAddress.Any, port);
    }
    
    public void Start()
    {
        AppLog.Startup(logger, port);
        try
        {
            LoopListening();
        }
        catch (Exception)
        {
            AppLog.RuntimeCriticalError(logger);
            listener.Close();
            throw;
        }
    }

    private void LoopListening()
    {
        while (true)
        {
            var command = listener.Receive(ref lastEndpoint);
            
            var isSuccessfulProcessing = ProcessCommand(command);

            if (IsConnected(lastEndpoint) || !isSuccessfulProcessing) continue;
            connectedUsers.Add(lastEndpoint);
            AppLog.UserConnected(logger, lastEndpoint.Address.ToString());
        }
    }

    private bool ProcessCommand(byte[] command)
    {
        if (command.Length < 4)
        {
            AppLog.IllegalCommandSize(logger, command.Length);
            return false;
        }
        
        var commandName = BitConverter.ToInt32(command, 0);
        switch (commandName)
        {
            case (int)Command.Ping:
                Send(lastEndpoint, "Pong!");
                return true;
            case (int)Command.VoiceData:
                VoiceDataCommand(command);
                return true;
            default:
                AppLog.UnknownCommand(logger, commandName);
                return false;
        }
    }

    private void VoiceDataCommand(byte[] command)
    {
        var endPoints = new IPEndPoint[connectedUsers.Count];
        connectedUsers.CopyTo(endPoints);
        var commandBody = command.Skip(4).ToArray();
        foreach (var endPoint in endPoints)
        {
            if (endPoint.Port.Equals(lastEndpoint.Port)) continue;
            Send(endPoint, commandBody);
        }
    }

    private void Send(IPEndPoint endPoint, string data)
    {
        var bytes = Encoding.ASCII.GetBytes(data);
        Send(endPoint, bytes);
    }
    
    private void Send(IPEndPoint endPoint, byte[] data)
    {
        listener.Send(data, data.Length, endPoint);
    }
    
    private bool IsConnected(IPEndPoint endPoint) => connectedUsers.Any(user => user.Port.Equals(endPoint.Port));
}