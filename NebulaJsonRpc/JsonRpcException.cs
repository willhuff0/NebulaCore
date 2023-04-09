using System.Text.Json.Nodes;

namespace NebulaJsonRpc;

public class JsonRpcException : Exception
{
    public int Code;
    public string Message;
    public object? Data;
    
    public JsonRpcException(int code, string message, object? data = null) : base(message)
    {
        Code = code;
        Message = message;
        Data = data;
    }

    public JsonObject Serialize(object? request)
    {
        dynamic modifiedData;
        if (Data is Dictionary<string, dynamic?> data && data.ContainsKey("request"))
        {
            modifiedData = data;
            modifiedData["request"] = request;
        }
        else if (Data == null)
        {
            modifiedData = new Dictionary<string, dynamic?>() { {"request", request} };
        }
        else
        {
            modifiedData = Data;
        }

        var id = request is Dictionary<string, dynamic?> _request ? _request["id"] : null;
        if (id is not string && id is not int) id = null;
        return new JsonObject()
        {
            {
                "jsonrpc", "2.0"
            },
            {
                "error", new JsonObject()
                {
                    { "code", Code },
                    { "message", Message },
                    { "data", modifiedData },
                }
            },
            {"id", id},
        };
    }
}