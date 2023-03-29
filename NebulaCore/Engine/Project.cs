using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NebulaCore.Engine;

public abstract class Project
{
    [JsonIgnore]
    public string Root { get; private set; }
    [JsonInclude, JsonPropertyName("bundle")]
    public AssetBundle<IAssetDefinitions> Bundle { get; private set; }

    // public JsonObject Serialize()
    // {
    //     return new JsonObject()
    //     {
    //         {"bundle", Bundle.Serialize()},
    //     };
    // }

    public static Project Deserialize(JsonObject json)
    {
        throw new NotImplementedException();
    }
}