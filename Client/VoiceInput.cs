using Client.Log;
using Microsoft.Extensions.Logging;
using NAudio.Wave;

namespace Client;

public class VoiceInput 
{
    private readonly ClientNetwork _clientNetwork;
    private readonly WaveInEvent _waveIn;
    private readonly ILogger _logger;

    public VoiceInput(ILoggerFactory factory, ClientNetwork clientNetwork, WaveFormat waveFormat)
    {
        _logger = factory.CreateLogger<VoiceClient>();
        _clientNetwork = clientNetwork;
        _waveIn = new WaveInEvent
        {
            DeviceNumber = 0,
            WaveFormat = waveFormat,
            BufferMilliseconds = 20
        };
        _waveIn.DataAvailable += OnDataAvailable;
    }
    
    public void Start()
    {
        _waveIn.StartRecording();
        VoiceInputLog.Start(_logger);
    }
    
    private void OnDataAvailable(object sender, WaveInEventArgs waveInEventArgs)
    {
        _clientNetwork.Send(BitConverter.GetBytes(1).Concat(waveInEventArgs.Buffer).ToArray());
    }
}