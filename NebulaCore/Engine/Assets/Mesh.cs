using System.Text.Json.Nodes;
using NebulaCore.Angle;

namespace NebulaCore.Engine.Assets;

[AssetDefinition("meshes")]
public class Mesh : FileAsset
{
    private int _indexCount;
    
    public Mesh(Project project, JsonNode json) : base(project, json)
    {
        _indexCount = json["indexCount"]!.GetValue<int>();
    }

    public override JsonObject Serialize()
    {
        var json = base.Serialize();
        json["indexCount"] = JsonValue.Create(_indexCount);
        return json;
    }

    public override Task<RuntimeAsset?> Load()
    {
        var data = FileAssetGetBytes();
        
        const int bytesPerIndex = 2; // UInt16
        var indexData = data.Take(_indexCount * bytesPerIndex);
        var vertexData = data.Skip(_indexCount * bytesPerIndex);
        
        var vao = GL.
    }
}

public class RuntimeMesh : RuntimeAsset
{
    public RuntimeMesh(Project project) : base(project)
    {
    }

    public override Task Unload()
    {
        throw new NotImplementedException();
    }
}