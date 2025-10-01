using System.Net;
using System.Net.Sockets;
using System.Text;

using var udpClient = new UdpClient(IPEndPoint.Parse("127.0.0.1:7777"));
await using var fileStream = new FileStream(Guid.NewGuid().ToString(), FileMode.Create, FileAccess.Write);

while (true)
{
    var result = await udpClient.ReceiveAsync();

    ShowUdpResultInfo(result);

    if (IsEnd(result.Buffer))
    {
        fileStream.Close();

        break;
    }

    fileStream.Write(result.Buffer);
}

Console.Read();

void ShowUdpResultInfo(UdpReceiveResult udpResult)
{
    Console.WriteLine($"remote: {udpResult.RemoteEndPoint.Address.ToString()}:{udpResult.RemoteEndPoint.Port}");
    Console.WriteLine($"buffer size: {udpResult.Buffer.Length}");
}

bool IsEnd(byte[] buffer)
{
    return buffer.Length == 4 && Encoding.UTF8.GetString(buffer) == "DONE";
}