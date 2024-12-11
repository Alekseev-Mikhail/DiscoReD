using System.Net;
using System.Net.Sockets;
using Client.Log;
using Microsoft.Extensions.Logging;

namespace Client;

public class ClientNetwork(ILoggerFactory factory, VoiceOutput voiceOutput, string address, int port)
{
    private readonly Socket _socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private readonly IPEndPoint _serverEndPoint = new(IPAddress.Parse(address), port);
    private readonly ILogger _logger = factory.CreateLogger<ClientNetwork>();

    public void Send(byte[] buffer) => _socket.SendToAsync(buffer, _serverEndPoint);

    public void StartListen(int receiveBufferSize)
    {
        ClientNetworkLog.StartListen(_logger, receiveBufferSize);
        var buffer = new byte[receiveBufferSize];
        while (true)
        {
            _socket.Receive(buffer);
            voiceOutput.AddVoice(buffer);
        }
    }
}