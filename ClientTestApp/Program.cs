using System.Net;
using System.Net.Sockets;
using System.Text;

var myClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
var serverEndpoint = IPEndPoint.Parse("127.0.0.1:7777");

myClientSocket.Connect(serverEndpoint);

while (true)
{
    var message = Console.ReadLine()!;

    if (message == "exit")
    {
        break;
    }

    var bytesToSend = Encoding.UTF8.GetBytes(message);

    myClientSocket.Send(bytesToSend);
}