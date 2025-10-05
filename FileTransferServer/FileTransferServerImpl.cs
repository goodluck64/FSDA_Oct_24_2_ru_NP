using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FileTransferServer;

internal sealed class FileTransferServerImpl : IFileTransferServer
{
    private readonly TcpListener _listener;
    private bool _isListening;
    private TcpClient? _client;

    public FileTransferServerImpl(IPEndPoint endPoint)
    {
        _listener = new TcpListener(endPoint);
    }

    public async Task StartAsync()
    {
        if (!_isListening)
        {
            _listener.Start();
            _isListening = true;
            _client = await _listener.AcceptTcpClientAsync();

            Console.WriteLine("Accepted!");

            await Task.Run(() =>
            {
                string? path = null;
                using var stream = _client.GetStream();
                using var streamReader = new StreamReader(stream);
                using var streamWriter = new StreamWriter(stream);
                using var binaryWriter = new BinaryWriter(stream);
                FileStream? fileStream = null;

                byte[] buffer = new byte[1024];

                while (_isListening) // TODO: replace with CancellationToken
                {
                    if (path is not null)
                    {
                        var read = fileStream!.Read(buffer);

                        Console.WriteLine($"Read: {read}");

                        if (read <= 0)
                        {
                            streamWriter.Write("DONE");
                            streamWriter.Flush();

                            break;
                        }

                        binaryWriter.Write(buffer, 0, read);
                        binaryWriter.Flush();
                    }
                    else
                    {
                        var result = streamReader.ReadLine();

                        if (IsDownloadAction(result, out path))
                        {
                            fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                            Console.WriteLine($"Requested path: {path}");
                        }
                    }
                }

                fileStream?.Dispose();
            });
        }
    }

    private const string DownloadAction = "DOWNLOAD";

    public static bool IsDownloadAction(string? action, [NotNullWhen(true)] out string? path)
    {
        path = null;

        if (action is null)
        {
            return false;
        }

        try
        {
            var split = action.Split(' ');

            if (split.First().ToUpper() == DownloadAction)
            {
                path = split.Last();

                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public Task StopAsync()
    {
        if (_isListening)
        {
            _listener.Stop();
        }

        return Task.CompletedTask;
    }
}

// DOWNLOAD C:\file.mp3 -> ["DOWNLOAD", "C:\file.mp3"]