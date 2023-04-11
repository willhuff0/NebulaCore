using System.Text.Json.Nodes;

namespace NebulaJsonRpc;

public class JsonRpcPeer : IJsonRpcServer, IJsonRpcClient
{
    private readonly JsonRpcServer _server;
    private readonly JsonRpcClient _client;
    
    public JsonRpcPeer(EventHandler<JsonObject> sendNetworkMessage, bool useValidation = false)
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

    public event EventHandler<JsonObject> SendNetworkMessage;
    public void ReceiveNetworkMessage(JsonObject message)
    {
        if (message.ContainsKey("result") || message.ContainsKey("error"))
        {
            _client.ReceiveNetworkMessage(message);
        }
        else
        {
            _server.ReceiveNetworkMessage(message);
        }
    }

    public void RegisterMethod(string method, Func<JsonNode?, Task<JsonNode?>?> callback)
    {
        _server.RegisterMethod(method, callback);
    }

    public Task<JsonNode> SendRequest(string method, params object[] param)
    {
        return _client.SendRequest(method, param);
    }

    public void SendNotification(string method, params object[] param)
    {
        _client.SendNotification(method, param);
    }
}