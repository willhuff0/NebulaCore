using System.Text.Json.Nodes;

namespace NebulaJsonRpc;

public class JsonRpcPeer : IJsonRpcServer, IJsonRpcClient
{
    private readonly JsonRpcServer _server;
    private readonly JsonRpcClient _client;
    
    public JsonRpcPeer(EventHandler<SendNetworkMessageEventArgs> sendNetworkMessage, bool useValidation = false)
    {
        SendNetworkMessage = sendNetworkMessage;

        _server = new JsonRpcServer(sendNetworkMessage, useValidation);
        _client = new JsonRpcClient(sendNetworkMessage, useValidation);
        
        IsOpen = true;
    }
    
    public bool IsOpen { get; private set; }
    
    public void Open()
    {
        IsOpen = true;
        _server.Open();
        _client.Open();
    }

    public void Close()
    {
        IsOpen = false;
        _server.Close();
        _client.Close();
    }

    public event EventHandler<SendNetworkMessageEventArgs> SendNetworkMessage;
    public void ReceiveNetworkMessage(SendNetworkMessageEventArgs args)
    {
        if (args.Message.ContainsKey("result") || args.Message.ContainsKey("error"))
        {
            _client.ReceiveNetworkMessage(args);
        }
        else
        {
            _server.ReceiveNetworkMessage(args);
        }
    }

    public void RegisterMethod(string method, Func<JsonNode?, JsonNode?> callback) => _server.RegisterMethod(method, callback);
    public void RegisterMethod(string method, Func<JsonNode?, Task<JsonNode?>> callback) => _server.RegisterMethod(method, callback);
    public void RegisterMethod(string method, Func<JsonNode?, Task> callback) => _server.RegisterMethod(method, callback);
    public void RegisterMethod(string method, Func<JsonNode?> callback) => _server.RegisterMethod(method, callback);
    public void RegisterMethod(string method, Func<Task<JsonNode?>> callback) => _server.RegisterMethod(method, callback);
    public void RegisterMethod(string method, Func<Task> callback) => _server.RegisterMethod(method, callback);
    public void RegisterMethod(string method, Action<JsonNode?> callback) => _server.RegisterMethod(method, callback);
    public void RegisterMethod(string method, Action callback) => _server.RegisterMethod(method, callback);

    public Task<JsonNode> SendRequest(string method, params object[] param)
    {
        return _client.SendRequest(method, param);
    }

    public void SendNotification(string method, params object[] param)
    {
        _client.SendNotification(method, param);
    }
}