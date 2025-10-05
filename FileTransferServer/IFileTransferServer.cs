namespace FileTransferServer;

internal interface IFileTransferServer
{
    Task StartAsync();
    Task StopAsync();
}