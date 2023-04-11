using System.Collections;
using System.Text.Json.Nodes;

namespace NebulaJsonRpc;

public class JsonRpcServer : IJsonRpcServer
{
    private readonly bool _useValidation;

    private readonly Dictionary<string, Func<JsonNode?, Task<JsonNode?>?>> _methods = new();

    public JsonRpcServer(EventHandler<JsonObject> sendNetworkMessage, bool useValidation = false)
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

    public event EventHandler<JsonObject> SendNetworkMessage;
    public async void ReceiveNetworkMessage(JsonObject message)
    {
        if (!IsOpen) return;
        var result = await _handleRequest(message);
        if (IsOpen && result != null)
        {
            SendNetworkMessage(this, result);
        }
    }

    public void RegisterMethod(string method, Func<JsonNode?, Task<JsonNode?>?> callback)
    {
        _methods.Add(method, callback);
    }

    private async Task<JsonObject?> _handleRequest(JsonObject request)
    {
        try
        {
            if (_useValidation) _validateRequest(request);

            var method = request["method"]!.GetValue<string>();
            if (!_methods.TryGetValue(method, out var func))
            {
                return new JsonRpcException((int)JsonRpcErrorCode.MethodNotFound, "The requested method does not exist.").Serialize(request);
            }

            var param = request["params"];
            var task = func(param);
            var result = task == null ? null : await task;
            
            var id = request["id"]?.GetValue<object?>();
            if (id != null)
            {
                return new JsonObject()
                {
                    { "jsonrpc", "2.0" },
                    { "result", result },
                    { "id", JsonValue.Create(id) }
                };
            }
            
            return null;
        }
        catch (Exception exception)
        {
            if (exception is JsonRpcException jsonRpcException)
            {
                return jsonRpcException.Serialize(request);
            }
           
            return new JsonRpcException((int)JsonRpcErrorCode.ServerError, exception.Message, new JsonObject()
                {
                    {"full", exception.ToString()},
                    {"stack", exception.StackTrace},
                }
            ).Serialize(request);
        }
    }

    private void _validateRequest(JsonObject request)
    {
        var jsonrpc = request["jsonrpc"]?.GetValue<string>();
        if (jsonrpc == null) throw new JsonRpcException((int)JsonRpcErrorCode.InvalidRequest, "Expected a value for \"jsonrpc\".");
        if (jsonrpc != "2.0") throw new JsonRpcException((int)JsonRpcErrorCode.InvalidRequest, "\"jsonrpc\" MUST be exactly \"2.0\".");

        var method = request["method"]?.GetValue<string>();
        if (method == null) throw new JsonRpcException((int)JsonRpcErrorCode.InvalidRequest, "Expected a value for \"method\".");
        if (method.StartsWith("rpc.")) throw new JsonRpcException((int)JsonRpcErrorCode.InvalidRequest, "Method names that begin with the word rpc followed by a period character (U+002E or ASCII 46) are reserved for rpc-internal methods and extensions and MUST NOT be used for anything else.");

        var param = request["params"]?.GetValue<object?>();
        if (param != null && param is not IEnumerable && param is not IDictionary) throw new JsonRpcException((int)JsonRpcErrorCode.InvalidRequest, "If present,\nby-position: \"params\" MUST be an enumerable.\nby-name: \"params\" MUST be a dictionary.");
        
        var id = request["id"]?.GetValue<object?>();
        if (id != null && !id.IsNumber() && id is not string) throw new JsonRpcException((int)JsonRpcErrorCode.InvalidRequest, "\"id\" MUST contain a string, number, or null value if included.");
    }
}