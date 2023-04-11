using System.Text.Json.Nodes;

namespace NebulaJsonRpc;

public class JsonRpcException : Exception
{
    public readonly int Code;
    public new readonly string Message;
    public new readonly JsonObject? Data;
    
    public JsonRpcException(int code, string message, JsonObject? data = null) : base(message)
    {
        Code = code;
        Message = message;
        Data = data;
    }

    public JsonRpcException(JsonObject json)
    {
        Code = json["code"]!.GetValue<int>();
        Message = json["message"]!.GetValue<string>();
        Data = json["data"]?.AsObject();
    }

    public JsonObject Serialize(JsonObject request)
    {
        JsonObject data;
        if (Data != null && !Data.ContainsKey("request"))
        {
            data = Data;
            data["request"] = request;
        }
        else
        {
            data = Data ?? new JsonObject() { { "request", request } };
        }

        var result = new JsonObject()
        {
            { "jsonrpc", "2.0" },
            { "error", new JsonObject() 
                {
                    { "code", Code },
                    { "message", Message },
                    { "data", data },
                }
            },
        };
        
        var id = request["id"]?.GetValue<int>();
        if (id != null)
        {
            result.Add("id", id);
        }

        return result;
    }
}