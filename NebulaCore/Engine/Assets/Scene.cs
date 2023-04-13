using System.Text.Json.Nodes;

namespace NebulaCore.Engine.Assets;

[AssetDefinition("scenes")]
public class Scene : Asset
{
    private List<Guid>? _collections;
    private List<Node> _roots;

    public Scene(Project project, JsonNode json) : base(project, json)
    {
        _collections = json["collections"]?.GetValue<List<string>>()?.Select(Guid.Parse).ToList();
        _roots = json["roots"]?.AsArray().Select(value => new Node(Project, value!)).ToList() ?? new List<Node>();
    }

    public override JsonObject Serialize()
    {
        throw new NotImplementedException();
    }

    public override async Task<RuntimeAsset?> Load()
    {
        if (_collections != null) await Task.WhenAll(_collections.Select(value => Project.Bundle.Load(value)));
        else await Project.Bundle.Load();
        var runtimeRoots = (await Task.WhenAll(_roots.Select(node => node.Load()))).Where(value => value != null);
        return new RuntimeScene(Project, this, )
    }
}

public class RuntimeScene : RuntimeAsset
{
    private List<RuntimeNode> _roots;

    public RuntimeScene(Project project, Asset from, List<RuntimeNode> roots) : base(project, from)
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