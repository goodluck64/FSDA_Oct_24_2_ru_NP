using System.Net;
using FileTransferServer;

var serverEndpoint = IPEndPoint.Parse("172.16.0.201:9999");
var fileTransferServer = new FileTransferServerImpl(serverEndpoint);

await fileTransferServer.StartAsync();

Console.WriteLine("Done!");