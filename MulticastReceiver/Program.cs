using System.Net;
using System.Net.Sockets;
using System.Text;

var multicastAddress = IPEndPoint.Parse("231.123.12.7");
var udpClient = new UdpClient(7788);

udpClient.JoinMulticastGroup(multicastAddress.Address);

while (true)
{
    var result = await udpClient.ReceiveAsync();

    Console.WriteLine(Encoding.UTF8.GetString(result.Buffer));
}