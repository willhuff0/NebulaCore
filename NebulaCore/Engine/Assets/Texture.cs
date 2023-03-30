using System.Text.Json.Nodes;

namespace NebulaCore.Engine.Assets;

[AssetDefinition("textures")]
public class Texture : FileAsset
{
    public Texture(Project project, JsonNode json) : base(project, json)
    {
        
    }

    public override Task<RuntimeAsset?> Load()
    {
        
    }

    public override JsonObject Serialize()
    {
        
    }
}

public class RuntimeTexture : RuntimeAsset
{
    private uint _handle;
    
    public RuntimeTexture(Project project, uint handle) : base(project)
    {
        _handle = handle;
    }

    public override Task Unload()
    {
        
    }
}