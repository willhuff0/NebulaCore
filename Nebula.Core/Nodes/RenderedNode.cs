using Nebula.Core.Assets;

namespace Nebula.Core.Nodes;

public class RenderedNode : Node
{
    public Mesh Mesh;
    public Material Material;
    
    public RenderedNode(Node? parent, string name, Mesh mesh, Material material) : base(parent, name)
    {
        Mesh = mesh;
        Material = material;
    }

    protected override List<Guid> GatherDependencies() => new() { Mesh.Key, Material.Key };

    protected override void OnRender(RenderArgs args)
    {
        if (!args.IsForShadows) Material.Bind();
        Mesh.Draw();
    }
}