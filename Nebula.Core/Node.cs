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

    public virtual void OnBeginFrame()
    {
        _children.ForEach(node => node.OnBeginFrame());
    }

    public virtual void OnRender()
    {
        _children.ForEach(node => node.OnRender());
    }

    public virtual void OnEndFrame()
    {
        _children.ForEach(node => node.OnEndFrame());
    }
}