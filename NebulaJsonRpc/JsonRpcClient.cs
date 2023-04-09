using System.Text.Json.Nodes;

namespace NebulaJsonRpc;

public class JsonRpcClient
{
    private Action<JsonObject> _sendMessage;
        
    private int _id = 0;
    
    //private int _pendingRequests;

    private bool _isOpen;

    public JsonRpcClient(Action<JsonObject> sendMessage)
    {
        _sendMessage = sendMessage;
        _isOpen = true;
    }

    public async void ReceiveMessage(JsonObject message)
    {
        
    }
}