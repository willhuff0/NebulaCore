using System.Text.Json.Nodes;

namespace NebulaCore.Engine.Assets;

[AssetDefinition("behaviors")]
public class BehaviorAsset : Asset
{
    private string _class;
    public string Class => _class;
    
    public BehaviorAsset(Project project, JsonNode json) : base(project, json)
    {
        _class = json["class"]!.GetValue<string>();
    }

    public override JsonObject Serialize() => new JsonObject()
    {
        { "class", _class }
    };

    public override Task<RuntimeAsset?> Load()
    {
        return Task.FromResult<RuntimeAsset?>(null);
    }
}