#include <iostream>
#include <WinSock2.h>
#include <ws2tcpip.h>
#include <thread>
#include <vector>

#pragma comment (lib, "Ws2_32.lib")

int main(int argc, char* argv[])
{
    WSADATA wsaData;
    auto wsaVersion = MAKEWORD(2, 2);

    if (int result = WSAStartup(wsaVersion, &wsaData); result != 0)
    {
        std::cout << "Failed to WSAStartup";

        return result;
    }

    addrinfo hints;

    ZeroMemory(&hints, sizeof(hints));
    
    hints.ai_family = AF_INET;
    hints.ai_socktype = SOCK_STREAM;
    hints.ai_protocol = IPPROTO_TCP;

    SOCKET serverSocket = socket(hints.ai_family, hints.ai_socktype, hints.ai_protocol);

    if (serverSocket == INVALID_SOCKET)
    {
        std::cout << "Failed to create server socket.";

        return -1;
    }

    addrinfo* outAddrInfo = nullptr;

    if (int result = getaddrinfo("172.20.208.162", "8989", &hints, &outAddrInfo); result != 0)
    {
        std::cout << "Failed to get address of server socket.";

        return result;
    }

    if (int result = bind(serverSocket, outAddrInfo->ai_addr, sizeof(addrinfo)); result != 0)
    {
        std::cout << "Failed to bind server socket.";

        return result;
    }

    if (int result = listen(serverSocket, SOMAXCONN); result != 0)
    {
        std::cout << "Failed to listen on socket.";

        return result;
    }

    std::vector<SOCKET> clients;
    std::vector<std::thread> clientThreads;

    std::thread mainThread{
        [serverSocket, &clientThreads, &clients]()
        {
            while (true)
            {
                std::cout << "Waiting for a client..." << '\n';

                SOCKET clientSocket = accept(serverSocket, nullptr, nullptr);
                std::cout << "Client accepted!" << '\n';

                if (clientSocket == INVALID_SOCKET)
                {
                    closesocket(clientSocket);
                    break;
                }
                                
                std::thread clientThread{
                    [clientSocket, &clients, &clientThreads]()
                    {
                        static constexpr int bufferLength = 1024;

                        std::unique_ptr<char[]> buffer{new char[bufferLength]};
                        
                        int bytesRead;

                        while ((bytesRead = recv(clientSocket, buffer.get(), bufferLength, 0)) != -1)
                        {
                            for (auto client : clients)
                            {
                                send(client, buffer.get(), bytesRead, 0);
                            }

                            std::memset(buffer.get(), 0, bufferLength);
                        }

                        closesocket(clientSocket);

                        std::erase_if(clients, [clientSocket](auto socket)
                        {
                            return socket == clientSocket;
                        });

                        std::cout << "Client disconnected!" << '\n';
                    }
                };
                

                clients.push_back(clientSocket);
                clientThreads.push_back(std::move(clientThread));
            }
        }
    };

    mainThread.join();
    
    WSACleanup();
    freeaddrinfo(outAddrInfo);

    return 0;
}
