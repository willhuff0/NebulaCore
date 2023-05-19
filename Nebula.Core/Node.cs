namespace Nebula.Core;

public abstract class Node
{
    public Node? Parent { get; private set; }
    public string Name { get; protected set; }

    private readonly List<Node> _children = new();
    public Node[] Children => _children.ToArray();

    protected Node(Node? parent, string name)
    {
        Parent = parent;
        Name = name;
    }

    public void SetParent(Node? node)
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

    internal List<Guid> _gatherDependencies() => _children.Aggregate(new List<Guid>(), (list, node) => list.Concat(node._gatherDependencies()).ToList()).Concat(GatherDependencies()).ToList();

    protected abstract List<Guid> GatherDependencies();

    protected virtual void OnBeginFrame(FrameArgs args) { }

    protected virtual void OnRender(RenderArgs args) { }

    protected virtual void OnEndFrame(FrameArgs args) { }
}