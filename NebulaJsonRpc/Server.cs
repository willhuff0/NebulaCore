using WatsonWebsocket;

namespace NebulaJsonRpc;

public class Server
{
    private WatsonWsServer _server;

    public Server(string host, int port)
    {
        _server = new WatsonWsServer(host, port);
        _server.MessageReceived += MessageReceived;
        _server.Start();
    }

    private void MessageReceived(object? sender, MessageReceivedEventArgs args)
    {
        
    }

    public void Stop()
    {
        _server.Stop();
        _server.Dispose();
    }
}