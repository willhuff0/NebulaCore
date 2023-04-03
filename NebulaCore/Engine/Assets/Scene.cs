using System.Text.Json.Nodes;

namespace NebulaCore.Engine.Assets;

[AssetDefinition("scenes")]
public class Scene : Asset
{
    public Scene(Project project, JsonNode json) : base(project, json)
    {
    }

    public override JsonObject Serialize()
    {
        throw new NotImplementedException();
    }

    public override Task<RuntimeAsset?> Load()
    {
        throw new NotImplementedException();
    }
}

public class RuntimeScene : RuntimeAsset
{
    private List<RuntimeNode> _roots;
    
    public RuntimeScene(Project project, List<RuntimeNode> roots) : base(project)
    {
        _roots = roots;
    }
    
    public async Task Load()
    {
        foreach (var node in _roots)
        {
            await node.Load();
        }
    }

    public void Frame(FrameEventArgs args)
    {
        foreach (var node in _roots)
        {
            node.Frame(args);
        }
    }

    public override async Task Unload()
    {
        foreach (var node in _roots)
        {
            await node.Unload();
        }
    }
}