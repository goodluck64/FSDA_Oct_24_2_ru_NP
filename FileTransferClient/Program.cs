using System.Net;
using System.Net.Sockets;
using System.Text;

var serverEndpoint = IPEndPoint.Parse("172.16.0.200:9999");
var tcpClient = new TcpClient();

await tcpClient.ConnectAsync(serverEndpoint);
var fileName = "OneDriveSetup.exe";
var networkStream = tcpClient.GetStream();
using var streamWriter = new StreamWriter(networkStream);
using var binaryReader = new BinaryReader(networkStream);
using var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);

streamWriter.WriteLine(@$"DOWNLOAD C:\Users\Alex\Desktop\{fileName}");
streamWriter.Flush();

var buffer = new byte[1024];

while (true)
{
    var read = binaryReader.Read(buffer);

    Console.WriteLine(read);
    
    if (read == 0 || IsDone(buffer, read))
    {
        fileStream.Flush();
        Console.WriteLine("DONE!");
        break;
    }
    
    fileStream.Write(buffer, 0, read);
}



bool IsDone(byte[] payload, int read)
{
    return read == 4 && Encoding.UTF8.GetString(payload, 0, read) == "DONE";
}

