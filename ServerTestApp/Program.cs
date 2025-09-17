using System.Net;
using System.Net.Sockets;
using System.Text;

var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
var myEndPoint = IPEndPoint.Parse("127.0.0.1:7777");
var manualResetEvent = new ManualResetEvent(false);

socket.Bind(myEndPoint);
socket.Listen();

Console.WriteLine("Listening...");
var clients = new List<Socket>();
_ = Task.Run(() =>
{
    while (true)
    {
        Console.WriteLine("Waiting for a client...");

        var clientSocket = socket.Accept();
        
        clients.Add(clientSocket);

        Console.WriteLine("Client accepted!");

        _ = Task.Run(() =>
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytes;

                while ((bytes = clientSocket.Receive(buffer)) != 0)
                {
                    var message = Encoding.UTF8.GetString(buffer);

                    Console.WriteLine($"Message from client: {message}");
                    
                    Array.Fill(buffer, (byte)0);
                }
            }
            finally
            {
                clientSocket.Dispose();
                clients.Remove(clientSocket);

                Console.WriteLine("Client disconnected!");
            }
        });
    }
});


manualResetEvent.WaitOne();