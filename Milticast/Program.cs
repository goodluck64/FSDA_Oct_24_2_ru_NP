// Unicast -> server -> one client || one client -> one client
// Multicast -> server -> many clients || one client -> many clients'
// Broadcast -> server -> all clients
//
//
//
// Subnet Mask = 255.255.255.0
//
//
// 172.20.208.1 - 172.20.208.254
//
// 172.20.208.255 - broadcast address


// 224.0.0.0 to 239.255.255.255 (multicast)

using System.Net;
using System.Net.Sockets;
using System.Text;

var multicastAddress = IPEndPoint.Parse("231.123.12.7:7788");
using var updClient = new UdpClient();

while (true)
{
    var message = Console.ReadLine()!;

    if (message == "exit")
    {
        break;
    }
    updClient.Send(Encoding.UTF8.GetBytes(message), multicastAddress);
}



    
    
