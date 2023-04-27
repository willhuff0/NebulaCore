using System.Collections;
using System.Text.Json.Nodes;

namespace Nebula.JsonRpc;

public interface IJsonRpcNetwork
{
    public bool IsOpen { get; }

    public void Open();
    public void Close();
    
    protected event EventHandler<SendNetworkMessageEventArgs> SendNetworkMessage;
    public void ReceiveNetworkMessage(SendNetworkMessageEventArgs args);
}

public class SendNetworkMessageEventArgs : EventArgs
{
    public JsonObject Message;
    public Guid? Client;

    public SendNetworkMessageEventArgs(JsonObject message, Guid? client = null)
    {
        Message = message;
        Client = client;
    }
}

public interface IJsonRpcServer : IJsonRpcNetwork
{
    public void RegisterMethod(string method, Func<JsonNode?, JsonNode?> callback);
    public void RegisterMethod(string method, Func<JsonNode?, Task<JsonNode?>> callback);
    public void RegisterMethod(string method, Func<JsonNode?, Task> callback);
    public void RegisterMethod(string method, Func<JsonNode?> callback);
    public void RegisterMethod(string method, Func<Task<JsonNode?>> callback);
    public void RegisterMethod(string method, Func<Task> callback);
    public void RegisterMethod(string method, Action<JsonNode?> callback);
    public void RegisterMethod(string method, Action callback);
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