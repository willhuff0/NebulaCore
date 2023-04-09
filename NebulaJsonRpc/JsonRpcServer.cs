using System.Text.Json.Nodes;

namespace NebulaJsonRpc;

public class JsonRpcServer
{
    private Action<JsonObject> _sendMessage;
    private Dictionary<string, Func<JsonNode?, Task<JsonNode?>?>> _methods;

    private bool _isOpen;
    
    public JsonRpcServer(Action<JsonObject> sendMessage, Dictionary<string, Func<JsonNode?, Task<JsonNode?>?>>? methods = null)
    {
        _sendMessage = sendMessage;
        _methods = methods ?? new Dictionary<string, Func<JsonNode?, Task<JsonNode?>?>>();
        _isOpen = true;
    }

    public void Close()
    {
        _isOpen = false;
    }

    public void RegisterMethod(string name, Func<JsonNode?, Task<JsonNode?>?> callback)
    {
        if (_methods.ContainsKey(name))
        {
            throw new ArgumentException($"There\'s already a method named {name}");
        }

        _methods[name] = callback;
    }

    public async void ReceiveMessage(JsonObject message)
    {
        var result = await _handleMessage(message);
        if (!_isOpen || result == null) return;
        _sendMessage(result);
    }

    private async Task<JsonObject?> _handleMessage(JsonObject request)
    {
        try
        {
            _validateRequest(request);

            var method = request["method"]!.GetValue<string>();
            var func = _methods[method];

            var result = await (func(request["params"]) ?? Task.FromResult<JsonNode?>(null));

            // If request has no id it is a notification and does not expect a response
            if (!request.ContainsKey("id")) return null;

            return new JsonObject()
            {
                { "jsonrpc", "2.0" },
                { "result", result },
                { "id", request["id"] }
            };
        }
        catch(Exception error)
        {
            if (error is JsonRpcException jsonRpcException)
            {
                return jsonRpcException.Serialize(request);
            }

            return new JsonRpcException(
                (int)JsonRpcErrorCode.ServerError,
                error.Message,
                new JsonObject()
                {
                    {"full", error.ToString()},
                    {"stack", error.StackTrace},
                }
            ).Serialize(request);
        }
    }

    private void _validateRequest(JsonObject request)
    {
        if (!request.ContainsKey("jsonrpc"))
            throw new JsonRpcException(
                (int)JsonRpcErrorCode.InvalidRequest,
                "Request must contain a \"jsonrpc\" key.");

        var jsonrpc = request["jsonrpc"]!.GetValue<string>();
        if (jsonrpc != "2.0")
            throw new JsonRpcException(
                (int)JsonRpcErrorCode.InvalidRequest,
                $"Invalid JSON-RPC version {jsonrpc}, expected \"2.0\".");

        if (!request.ContainsKey("method"))
            throw new JsonRpcException(
                (int)JsonRpcErrorCode.InvalidRequest,
                "Request must contain a \"method\" key.");
    }
}