using System.Net;
using System.Net.Sockets;
using System.Text;
using Client.Log;
using Core;
using Microsoft.Extensions.Logging;

namespace Client;

public class ClientNetwork(ILoggerFactory factory, VoiceOutput voiceOutput, string address, int port)
{
    private readonly Socket _socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private readonly IPEndPoint _serverEndPoint = new(IPAddress.Parse(address), port);
    private readonly ILogger _logger = factory.CreateLogger<ClientNetwork>();
    private byte[] _clientId = new byte[4];
    
    public void Send(CommandName name, byte[] body) =>
        _socket.SendToAsync(ConvertToRawCommand(name, _clientId, body), _serverEndPoint);

    public byte[] SendBodiless(CommandName commandName, int bufferSize)
    {
        ClientNetworkLog.SendingBodilessCommand(_logger, commandName.ToString());
        _socket.SendToAsync(ConvertToRawCommand(commandName, _clientId), _serverEndPoint);

        var buffer = new byte[bufferSize];
        _socket.Receive(buffer);
        return buffer;
    }

    public void Connect()
    {
        _clientId = SendBodiless(CommandName.Ping, 4);
        ClientNetworkLog.Pong(_logger);
    }

    public void StartListen(int bufferSize)
    {
        ClientNetworkLog.StartListen(_logger, bufferSize);
        var buffer = new byte[bufferSize];
        while (true)
        {
            try
            {
                _socket.Receive(buffer);
                voiceOutput.AddVoice(buffer);
            }
            catch (Exception e)
            {
                ClientNetworkLog.RuntimeWarning(_logger, e.Message, e.StackTrace);
            }
        }
    }

    private static byte[] ConvertToRawCommand(CommandName commandName, byte[] clientId) => 
        BitConverter.GetBytes((int)commandName).Concat(clientId).ToArray();
    
    private static byte[] ConvertToRawCommand(CommandName commandName, byte[] clientId, byte[] body) => 
        BitConverter.GetBytes((int)commandName).Concat(clientId).Concat(body).ToArray();
}