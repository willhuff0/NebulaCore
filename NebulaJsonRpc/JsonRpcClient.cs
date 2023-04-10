using System.Text.Json.Nodes;

namespace NebulaJsonRpc;

public class JsonRpcClient
{
    private Action<JsonObject> _sendMessage;
        
    private int _id = -1;
    
    private Dictionary<int, TaskCompletionSource<JsonNode?>> _pendingRequests = new();

    private bool _isOpen;

    public JsonRpcClient(Action<JsonObject> sendMessage)
    {
        _sendMessage = sendMessage;
        _isOpen = true;
    }

    public void ReceiveMessage(JsonObject message)
    {
        var id = message["id"].GetValue<int>();
        if (id == null) return;

        _pendingRequests.Remove(id, out var completer);
        if (completer == null) return;
        
        completer.SetResult();
    }

    public async Task<JsonNode> SendRequest(string method, params object[] param)
    {
        var id = _id++;
        var message = new JsonObject()
        {
            { "jsonrpc", "2.0" },
            {"id", id},
            { "method", method },
        };
        
        if (param is [Dictionary<string, object> paramsByName])
        {
            message.Add("params", JsonValue.Create(paramsByName));
        }
        else if (param.Length > 0)
        {
            message.Add("params", JsonValue.Create(param));
        }
        
        _sendMessage(message);

        var completer = new TaskCompletionSource<JsonNode?>();
        _pendingRequests.Add(id, completer);

        return (await completer.Task)!;
    }
}