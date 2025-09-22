var tcpChat = new TcpChatServer();

tcpChat.OnClientConnected += () => { Console.WriteLine("Client connected!"); };

tcpChat.OnClientDisconnected += () => { Console.WriteLine("Client disconnected!"); };

tcpChat.OnStartedListening += () => { Console.WriteLine("Started listening!"); };

await tcpChat.Start();