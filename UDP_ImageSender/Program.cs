using System.Net;
using UDP_ImageServer;

var path = OpenFileDialog.Show();

using var sender = new UdpImageSender(path);

await sender.SendFileAsync(IPEndPoint.Parse("127.0.0.1:7777"));

Console.Read();