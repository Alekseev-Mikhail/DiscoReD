using Client;
using Microsoft.Extensions.Logging;

var factory = LoggerFactory.Create(builder => builder.AddConsole());
var clientInit = new VoiceClient(factory, "192.168.1.125", 11000);
clientInit.ClientNetwork.Connect();
clientInit.VoiceInput.Start();
clientInit.VoiceOutput.Start();
Thread.Sleep(1000);
clientInit.ClientNetwork.StartListen(3528);