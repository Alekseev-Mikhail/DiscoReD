using System.Net;
using System.Net.Sockets;
using System.Text;
using Core;
using Microsoft.Extensions.Logging;

namespace Server;

public class Guild
{
    private readonly int _port;
    private readonly UdpClient _listener;
    private readonly List<IPEndPoint> _connectedUsers = [];
    private readonly ILogger _logger;
    private IPEndPoint _lastEndPoint;

    public Guild(ILoggerFactory factory, string portAsString)
    {
        _logger = factory.CreateLogger<Guild>();

        try
        {
            _port = int.Parse(portAsString);
        }
        catch (Exception)
        {
            ServerLog.StartupCriticalError(_logger, portAsString);
            throw;
        }

        _listener = new UdpClient(_port);
        _lastEndPoint = new IPEndPoint(IPAddress.Any, _port);
    }

    public void Start()
    {
        ServerLog.Startup(_logger, _port);
        while (true)
        {
            try
            {
                ListenNext();
            }
            catch (Exception e)
            {
                if (e is SocketException)
                {
                    ServerLog.ClientWasDisconnectedWhileAccessing(_logger, _lastEndPoint.Address.ToString());
                    continue;
                }
                ServerLog.RuntimeCriticalError(_logger, e.StackTrace);
            }
        }
    }

    private void ListenNext()
    {
        var command = _listener.Receive(ref _lastEndPoint);
        
        var isSuccessfulProcessing = ProcessCommand(command);

        if (IsConnected(_lastEndPoint) || !isSuccessfulProcessing) return;
        _connectedUsers.Add(_lastEndPoint);
        ServerLog.UserConnected(_logger, _lastEndPoint.Address.ToString());
    }

    private bool ProcessCommand(byte[] command)
    {
        if (command.Length < 4)
        {
            ServerLog.IllegalCommandSize(_logger, command.Length);
            return false;
        }

        var commandName = BitConverter.ToInt32(command, 0);
        switch (commandName)
        {
            case (int)Command.Ping:
                Send(_lastEndPoint, "Pong!");
                return true;
            case (int)Command.VoiceData:
                VoiceDataCommand(command);
                return true;
            default:
                ServerLog.UnknownCommand(_logger, commandName);
                return false;
        }
    }

    private void VoiceDataCommand(byte[] command)
    {
        var endPoints = new IPEndPoint[_connectedUsers.Count];
        _connectedUsers.CopyTo(endPoints);
        var commandBody = command.Skip(4).ToArray();
        foreach (var endPoint in endPoints)
        {
            if (endPoint.Port.Equals(_lastEndPoint.Port)) continue;
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
        _listener.Send(data, data.Length, endPoint);
    }

    private bool IsConnected(IPEndPoint endPoint) => _connectedUsers.Any(user => user.Port.Equals(endPoint.Port));
}