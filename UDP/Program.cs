using System.Net;
using System.Net.Sockets;
using System.Text;


// 65535 - 527 = 65008

// var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

var serverIpEndpoint = IPEndPoint.Parse("127.0.0.1:7777");
var client = new UdpClient(serverIpEndpoint);
var manualResetEvent = new ManualResetEvent(false);


Task.Run(async () =>
{

    int prev = -1;
    while (true)
    {
        var result = await client.ReceiveAsync();
        var message = Encoding.UTF8.GetString(result.Buffer);
        var current = GetPacketNumber(message);
        
        // prev = 399
        // current = 400
        if (prev != current - 1)
        {
            Console.WriteLine("Loss");
        }
        
        prev = current;
        
        Console.WriteLine(message);
    }
});


Console.WriteLine(GetPacketNumber("[123456] Hello World!"));

int GetPacketNumber(string message)
{
    var charEnum = message.Skip(1).TakeWhile(x => x != ']');
    var stringBuilder = new StringBuilder();

    foreach (var character in charEnum)
    {
        stringBuilder.Append(character);
    }
    
    return int.Parse(stringBuilder.ToString());
}

manualResetEvent.WaitOne();

class Package
{
    public int Number { get; set; }
    public required string Message { get; init; }
}


