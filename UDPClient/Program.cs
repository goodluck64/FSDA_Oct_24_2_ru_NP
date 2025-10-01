using System.Net;
using System.Net.Sockets;
using System.Text;

var remoteIpEndpoint = IPEndPoint.Parse("127.0.0.1:7777");
var udpClient = new UdpClient();

while (true)
{
    string message = Console.ReadLine()!;

    for (int i = 0;; i++)
    {
        var bytes = Encoding.UTF8.GetBytes($"[{i}]{message}");

        //await Task.Delay(10);
        
        await udpClient.SendAsync(bytes, bytes.Length, remoteIpEndpoint);
    }
}