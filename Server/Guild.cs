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
    private readonly List<IPEndPoint> _connectedClients = [];
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
        ServerLog.Start(_logger, _port);
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
                    ServerLog.ClientIsNotAccessable(
                        _logger,
                        _lastEndPoint.Address.ToString(),
                        _connectedClients.Count
                    );
                    continue;
                }

                ServerLog.RuntimeWarning(_logger, e.Message, e.StackTrace);
            }
        }
    }

    private void ListenNext()
    {
        var rawCommand = _listener.Receive(ref _lastEndPoint);
        var command = Command.Parse(rawCommand);
        
        ProcessCommand(command);
    }

    private void ProcessCommand(Command command)
    {
        switch (command.Name)
        {
            case CommandName.Ping:
                PingCommand();
                break;
            case CommandName.VoiceData:
                VoiceDataCommand(command);
                break;
            default:
                ServerLog.UnknownCommand(_logger, command.Name.ToString());
                break;
        }
    }

    private void PingCommand()
    {
        _connectedClients.Add(_lastEndPoint);
        Send(_lastEndPoint, _connectedClients.Count - 1);
        ServerLog.UserConnected(_logger, _lastEndPoint.Address.ToString());
    }
    
    private void VoiceDataCommand(Command command)
    {
        var endPoints = new IPEndPoint[_connectedClients.Count];
        _connectedClients.CopyTo(endPoints);
        foreach (var endPoint in endPoints)
        {
            if (endPoint.Port.Equals(_lastEndPoint.Port)) continue;
            Send(endPoint, command.Body);
        }
    }

    private void Send(IPEndPoint endPoint, string data)
    {
        var bytes = Encoding.ASCII.GetBytes(data);
        Send(endPoint, bytes);
    }
    
    private void Send(IPEndPoint endPoint, int data)
    {
        var bytes = BitConverter.GetBytes(data);
        Send(endPoint, bytes);
    }

    private void Send(IPEndPoint endPoint, byte[] data)
    {
        _listener.Send(data, data.Length, endPoint);
    }
}