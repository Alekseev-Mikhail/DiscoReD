using Microsoft.Extensions.Logging;
using NAudio.Wave;

namespace Client;

public class VoiceClient
{
    public readonly WaveFormat StandardWaveFormat = new(rate: 44100, bits: 32, channels: 1);
    public readonly ClientNetwork ClientNetwork;
    public readonly VoiceInput VoiceInput;
    public readonly VoiceOutput VoiceOutput;

    public VoiceClient(ILoggerFactory factory, string address, int port)
    {
        ILogger logger = factory.CreateLogger<VoiceClient>();

        Log.VoiceClientLog.StartInit(logger, address, port);

        Log.VoiceClientLog.InitOf(logger, nameof(VoiceOutput));
        VoiceOutput = new VoiceOutput(factory, StandardWaveFormat);

        Log.VoiceClientLog.InitOf(logger, nameof(ClientNetwork));
        ClientNetwork = new ClientNetwork(factory, VoiceOutput, address, port);

        Log.VoiceClientLog.InitOf(logger, nameof(VoiceInput));
        VoiceInput = new VoiceInput(factory, ClientNetwork, StandardWaveFormat);

        Log.VoiceClientLog.FinishInit(logger);
    }
}