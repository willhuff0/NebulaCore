using System.Text.Json.Nodes;

namespace Nebula.Core;

public static class NodeLoaderDatabase
{
    public static readonly Dictionary<string, LoadNodeDelegate> Loaders = new();
    
    public static void ReloadNodeLoaders()
    {
        var loaders =
            from assembly in AppDomain.CurrentDomain.GetAssemblies().AsParallel()
            from type in assembly.GetTypes()
            let attributes = type.GetCustomAttributes(typeof(NodeLoaderAttribute), false)
            where attributes is { Length: > 0 }
            select new { Type = type, Attributes = attributes.Cast<NodeLoaderAttribute>() };

        foreach (var loader in loaders)
        {
            Loaders[loader.Attributes.First().Class] = loader.Type.GetMethod("LoadNode")!.CreateDelegate<LoadNodeDelegate>();
        }
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class NodeLoaderAttribute : Attribute
{
    public readonly string Class;

    public NodeLoaderAttribute(string @class)
    {
        Class = @class;
    }
}

public delegate Node? LoadNodeDelegate(Project project, string name, JsonNode? properties);

public interface INodeLoader
{
    public static abstract Node? LoadNode(Project project, string name, JsonNode? properties);
}

public abstract class Node
{
    public Node? Parent { get; set; }
    public string Name { get; protected set; }

    private readonly List<Node> _children = new();
    public Node[] Children => _children.ToArray();

    protected Node(Node? parent, string name)
    {
        Parent = parent;
        Name = name;
    }

    private void SetParent(Node? node)
    {
        Parent = node;
    }

    public void AttachChild(Node node)
    {
        node.SetParent(this);
        _children.Add(node);
    }

    public void DetachChild(int index)
    {
        _children[index].SetParent(null);
        _children.RemoveAt(index);
    }

    internal void _onBeginFrame(FrameArgs args)
    {
        _children.ForEach(node => node._onBeginFrame(args));
        OnBeginFrame(args);
    }

    internal void _onRender(RenderArgs args)
    {
        _children.ForEach(node => node._onRender(args));
        OnRender(args);
    }

    internal void _onEndFrame(FrameArgs args)
    {
        _children.ForEach(node => node._onEndFrame(args));
        OnEndFrame(args);
    }

    internal HashSet<Asset> _gatherDependencies() => _children.Aggregate(new HashSet<Asset>(), (list, node) => list.Concat(node._gatherDependencies()).ToHashSet()).Concat(GatherDependencies()).ToHashSet();

    protected abstract HashSet<Asset> GatherDependencies();

    protected virtual void OnBeginFrame(FrameArgs args) { }

    protected virtual void OnRender(RenderArgs args) { }

    protected virtual void OnEndFrame(FrameArgs args) { }
}