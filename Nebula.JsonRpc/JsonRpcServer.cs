﻿using System.Collections;
using System.Text.Json.Nodes;

namespace Nebula.JsonRpc;

public class JsonRpcServer : IJsonRpcServer
{
    private readonly bool _useValidation;

    private readonly Dictionary<string, Func<JsonNode?, Task<JsonNode?>?>> _methods = new();

    public JsonRpcServer(EventHandler<SendNetworkMessageEventArgs> sendNetworkMessage, bool useValidation = false)
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
    public async void ReceiveNetworkMessage(SendNetworkMessageEventArgs args)
    {
        if (!IsOpen) return;
        var result = await _handleRequest(args.Message);
        if (IsOpen && result != null)
        {
            SendNetworkMessage(this, new SendNetworkMessageEventArgs(result, args.Client));
        }
    }

    public void RegisterMethod(string method, Func<JsonNode?, JsonNode?> callback)
    {
        _methods.Add(method, param => Task.FromResult<JsonNode?>(callback(param)));
    }

    public void RegisterMethod(string method, Func<JsonNode?, Task<JsonNode?>> callback)
    {
        _methods.Add(method, callback!);
    }

    public void RegisterMethod(string method, Func<JsonNode?, Task> callback)
    {
        _methods.Add(method, async param =>
        {
            await callback(param);
            return null;
        });
    }

    public void RegisterMethod(string method, Func<JsonNode?> callback)
    {
        _methods.Add(method, _ => Task.FromResult<JsonNode?>(callback()));
    }

    public void RegisterMethod(string method, Func<Task<JsonNode?>> callback)
    {
        _methods.Add(method, _ => callback()!);
    }

    public void RegisterMethod(string method, Func<Task> callback)
    {
        _methods.Add(method, async _ =>
        {
            await callback();
            return null;
        });
    }

    public void RegisterMethod(string method, Action<JsonNode?> callback)
    {
        _methods.Add(method, param =>
        {
            callback(param);
            return Task.FromResult<JsonNode?>(null);
        });
    }

    public void RegisterMethod(string method, Action callback)
    {
        _methods.Add(method, _ =>
        {
            callback();
            return Task.FromResult<JsonNode?>(null);
        });
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