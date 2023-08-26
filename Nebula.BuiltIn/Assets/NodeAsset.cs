using System.Text.Json.Nodes;
using Nebula.Core;

namespace Nebula.BuiltIn.Assets;

[AssetLoader("node")]
public class NodeAsset : Asset, IAssetLoader
{
    public static Asset? LoadAsset(Project project, AssetDefinition definition, bool immediateEnterScope)
    {
        var @class = definition.Properties!["class"]!.GetValue<string>();
        var name = definition.Properties!["name"]!.GetValue<string>();
        var properties = definition.Properties!["properties"];
        
        var parent = definition.Properties!["parent"]?.GetValue<string?>();
        Guid? parentGuid = parent != null ? Guid.Parse(parent) : null;

        var children = definition.Properties!["children"]?.AsArray().Select(child => Guid.Parse(child!.GetValue<string>())).ToList() ?? new List<Guid>();

        return new NodeAsset(project.Database, definition.Type, definition.Key, (parentGuid, children), @class, name, properties);
    }

    private readonly string _class;
    private readonly string _name;
    private readonly JsonNode? _properties; 

    public NodeAsset(AssetDatabase database, string type, Guid key, (Guid? parent, List<Guid> children)? referenceIntermediary, string @class, string name, JsonNode? properties) : base(database, type, key)
    {
        _referenceIntermediary = referenceIntermediary;
        _class = @class;
        _name = name;
        _properties = properties;
    }

    private (Guid? parent, List<Guid> children)? _referenceIntermediary;

    private NodeAsset? _parent = null!;
    private List<NodeAsset> _children = null!;

    protected override HashSet<Asset> GatherDependencies()
    {
        var dependencies = _children.SelectMany(child => child.Dependencies);
        if (_parent != null) dependencies = dependencies.Append(_parent);
        return dependencies.ToHashSet();
    }

    protected HashSet<Asset> GatherAllDependencies()
    {
        // Add dependencies from specific node class
        
        var dependencies = _children.SelectMany(child => child.Dependencies);
        if (_parent != null) dependencies = dependencies.Append(_parent);
        return dependencies.ToHashSet();
    }

    public override void LookupReferences()
    {
        var parent = _referenceIntermediary!.Value.parent;
        if (parent != null) _parent = Database.GetLoadedAsset<NodeAsset>(parent.Value);

        _children = _referenceIntermediary!.Value.children.Select(child => Database.GetLoadedAsset<NodeAsset>(child)!).ToList();

        _referenceIntermediary = null;
    }

    public TNode? Instantiate<TNode>() where TNode : Node
    {
        var root = NodeLoaderDatabase.Loaders[_class](Database.AssociatedProject, _name, _properties);
        if (root == null) return null;

        foreach (var child in _children)
        {
            var node = child.Instantiate<Node>();
            if (node != null) root.AttachChild(node);
        }

        return (TNode)root;
    }
}