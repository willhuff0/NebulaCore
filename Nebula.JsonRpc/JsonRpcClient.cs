using System.Collections;
using System.Text.Json.Nodes;

namespace Nebula.JsonRpc;

public class JsonRpcClient : IJsonRpcClient
{
    private readonly bool _useValidation;

    private int _nextId = 0;
    private readonly Dictionary<int, TaskCompletionSource<JsonNode>> _pendingRequests = new();

    public JsonRpcClient(EventHandler<SendNetworkMessageEventArgs> sendNetworkMessage, bool useValidation = false)
    {
        SendNetworkMessage = sendNetworkMessage;
        _useValidation = useValidation;

        IsOpen = true;
    }

    public bool IsOpen { get; private set; }
    
    public void Open()
    {
        IsOpen = true;
    }

    public void Close()
    {
        IsOpen = false;
    }

    public event EventHandler<SendNetworkMessageEventArgs> SendNetworkMessage;
    public void ReceiveNetworkMessage(SendNetworkMessageEventArgs args)
    {
        if (!IsOpen) return;
        _handleResponse(args.Message);
    }

    public Task<JsonNode> SendRequest(string method, params object[] param)
    {
        var completer = new TaskCompletionSource<JsonNode>();
        _pendingRequests.Add(_nextId, completer);
        _send(_nextId, method, param);
        _nextId++;
        return completer.Task;
    }

    public void SendNotification(string method, params object[] param)
    {
        _send(null, method, param);
    }

    private void _send(int? id, string method, params object[] param)
    {
        var message = new JsonObject()
        {
            { "jsonrpc", "2.0" },
            { "method", method },
        };
        if (id != null) message.Add("id", id);

        if (param is [IDictionary paramByName])
        {
            message.Add("params", JsonValue.Create(paramByName));            
        }
        else if (param.Length > 0)
        {
            message.Add("params", JsonValue.Create(param));
        }

        SendNetworkMessage(this, new SendNetworkMessageEventArgs(message));
    }

    private void _handleResponse(JsonObject response)
    {
        if (_useValidation) _validateResponse(response);

        var id = response["id"]?.GetValue<int>();
        if (id == null) return;

        _pendingRequests.Remove((int)id, out var completer);
        if (completer == null) return;

        var result = response["result"];
        if (result == null)
        {
            var error = response["error"]?.AsObject();
            if (error == null)
            {
                completer.SetException(new Exception("JsonRpc response is missing both \"result\" and \"error\"."));
                return;
            }
            
            completer.SetException(new JsonRpcException(error));
            return;
        }
        
        completer.SetResult(result);
    }

    private void _validateResponse(JsonObject response)
    {
        return; // Not sure if validation on client is needed. Also, could lead to cyclic errors.

        var jsonrpc = response["jsonrpc"]?.GetValue<string>();
        if (jsonrpc == null) throw new JsonRpcException((int)JsonRpcErrorCode.InvalidRequest, "Expected a value for \"jsonrpc\".");
        if (jsonrpc != "2.0") throw new JsonRpcException((int)JsonRpcErrorCode.InvalidRequest, "\"jsonrpc\" MUST be exactly \"2.0\".");

        var result = response["result"]?.GetValue<string>();
        var error = response["error"]?.GetValue<string>();
    }
}