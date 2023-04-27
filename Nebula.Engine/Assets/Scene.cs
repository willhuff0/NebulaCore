using System.Collections.ObjectModel;
using System.Text.Json.Nodes;

namespace Nebula.Engine.Assets;

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
        var json = new JsonObject();
        if (_roots.Count > 0)
        {
            json["roots"] = JsonValue.Create(_roots.Select(node => node.Serialize()));
        }
        if (_collections != null && _collections.Count > 0)
        {
            json["collections"] = JsonValue.Create(_collections.Select(guid => guid.ToString()));
        }
        return json;
    }

    public override async Task<RuntimeAsset?> Load()
    {
        if (_collections != null) await Task.WhenAll(_collections.Select(value => Project.Bundle.Load(value)));
        else await Project.Bundle.Load();
        var runtimeRoots = new List<RuntimeNode>();
        await Task.WhenAll(_roots.Select(async node =>
        {
            var runtimeRoot = await node.Load();
            if (runtimeRoot != null) runtimeRoots.Add((RuntimeNode)runtimeRoot);
        }));
        return new RuntimeScene(Project, this, runtimeRoots);
    }
}

public class RuntimeScene : RuntimeAsset
{
    public readonly String Name;
    private readonly List<RuntimeNode> _roots;

    public ReadOnlyCollection<RuntimeNode> Roots => new ReadOnlyCollection<RuntimeNode>(_roots);

    public RuntimeScene(Project project, Asset from, List<RuntimeNode> roots) : base(project, from)
    {
        Name = from.Name;
        _roots = roots;
    }

    public JsonObject Serialize()
    {
        var json = new JsonObject();
        json.Add("name", Name);
        json.Add("roots", JsonValue.Create(_roots.Select(runtimeNode => runtimeNode.Serialize())));
        return json;
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

    public Node? FindNode(Guid guid)
    {
        foreach (var node in _roots)
        {
            if (node)
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