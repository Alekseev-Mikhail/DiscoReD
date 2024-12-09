using Microsoft.Extensions.Logging;
using Server;

var factory = LoggerFactory.Create(builder => builder.AddConsole());
var server = new Guild(factory, args[0]);
server.Start();