using System.Net;
using System.Net.Sockets;

namespace UDP_ImageServer;

class UdpImageSender : IDisposable, IAsyncDisposable
{
    private const int BufferSize = short.MaxValue;
    private FileStream _fileStream;
    private BinaryReader _binaryReader;
    private UdpClient _udpClient;

    private byte[] _buffer = new byte[BufferSize];

    public UdpImageSender(string path)
    {
        _fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        _binaryReader = new BinaryReader(_fileStream);
        _udpClient = new UdpClient();
    }

    public async Task SendFileAsync(IPEndPoint receiver)
    {
        while (_fileStream.Position < _fileStream.Length)
        {
            _ = _binaryReader.Read(_buffer, 0, BufferSize);

            await _udpClient.SendAsync(_buffer, receiver);
            await Task.Delay(10);
        }

        await _udpClient.SendAsync("DONE"u8.ToArray(), receiver);
    }

    public void Dispose()
    {
        _fileStream.Dispose();
        _udpClient.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _fileStream.DisposeAsync();
    }
}