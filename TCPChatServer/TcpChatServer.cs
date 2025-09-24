using System.Net;
using System.Net.Sockets;

internal sealed class TcpChatServer : IDisposable
{
    private TcpListener _listener;
    private List<ChatClient> _clients = new();
    private CancellationTokenSource _cts = new();

    public event Action? OnClientConnected;
    public event Action? OnClientDisconnected;
    public event Action? OnStartedListening;

    public TcpChatServer()
    {
        _listener = new TcpListener(IPEndPoint.Parse("172.20.208.162:8989"));
    }

    public Task Start()
    {
        _listener.Start();

        return Task.Factory.StartNew(() =>
        {
            OnStartedListening?.Invoke();

            while (!_cts.IsCancellationRequested)
            {
                var client = _listener.AcceptTcpClient();
                var networkStream = client.GetStream();
                var chatClient = new ChatClient
                {
                    Client = client,
                    Writer = new StreamWriter(networkStream),
                    Reader = new StreamReader(networkStream)
                };
                
                _clients.Add(chatClient);

                Task.Run(() =>
                {
                    try
                    {
                        OnClientConnected?.Invoke();

                        while (true)
                        {
                            var message = chatClient.Reader.ReadLine();

                            foreach (var currentClient in _clients)
                            {
                                currentClient.Writer.WriteLine(message);
                                currentClient.Writer.Flush();
                            }
                        }
                    }
                    finally
                    {
                        _clients.Remove(chatClient);
                        OnClientDisconnected?.Invoke();
                    }
                });
            }
        }, TaskCreationOptions.LongRunning);
    }

    public void Stop()
    {
        _cts.Cancel();
    }

    public void Dispose()
    {
        _listener.Dispose();
        _cts.Dispose();
    }
}

internal sealed class ChatClient
{
    public required TcpClient Client { get; set; }
    public required StreamReader Reader { get; set; }
    public required StreamWriter Writer { get; set; }
}