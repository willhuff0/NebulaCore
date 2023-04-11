using System.Collections;
using System.Text.Json.Nodes;

namespace NebulaJsonRpc;

public interface IJsonRpcNetwork
{
    public bool IsOpen { get; }

    public void Open();
    public void Close();
    
    protected event EventHandler<JsonObject> SendNetworkMessage;
    public void ReceiveNetworkMessage(JsonObject message);
}

public interface IJsonRpcServer : IJsonRpcNetwork
{
    public void RegisterMethod(string method, Func<JsonNode?, Task<JsonNode?>?> callback);
}

public interface IJsonRpcClient : IJsonRpcNetwork
{
    public Task<JsonNode> SendRequest(string method, params object[] param);
    public void SendNotification(string method, params object[] param);
}

internal static class ObjectNumberExtensions
{
    public static bool IsNumber(this object value)
    {
        return value is sbyte
               || value is byte
               || value is short
               || value is ushort
               || value is int
               || value is uint
               || value is long
               || value is ulong
               || value is float
               || value is double
               || value is decimal;
    }
}