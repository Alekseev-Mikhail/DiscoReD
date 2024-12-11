using Client.Log;
using Microsoft.Extensions.Logging;
using NAudio.Wave;

namespace Client;

public class VoiceOutput(ILoggerFactory factory, WaveFormat waveFormat)
{
    private readonly WaveOutEvent waveOut = new();
    private readonly BufferedWaveProvider bufferedWaveProvider = new(waveFormat);
    private readonly ILogger _logger = factory.CreateLogger<VoiceOutput>();

    public void AddVoice(byte[] buffer)
    {
        bufferedWaveProvider.AddSamples(buffer, 0, buffer.Length);
    }

    public void Start()
    {
        waveOut.Init(bufferedWaveProvider);
        waveOut.Play();
        VoiceOutputLog.StartPlay(_logger);
    }
}