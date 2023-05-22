using Nebula.Core;
using Nebula.BuiltIn.Assets;
using Nebula.Math;

namespace Nebula.BuiltIn.Nodes;

public class RenderedNode : Node
{
    public Mesh Mesh;
    public Material Material;
    
    public RenderedNode(Node? parent, string name, Mesh mesh, Material material) : base(parent, name)
    {
        Mesh = mesh;
        Material = material;
    }

    protected override HashSet<Asset> GatherDependencies() => Mesh.Dependencies.Concat(Material.Dependencies).ToHashSet();

    protected override void OnRender(RenderArgs args)
    {
        if (!args.IsForShadows) Material.Bind();
        
        var projection = Matrix4.Perspective(args.FrameArgs.Aspect, 80.0 * (double.Pi / 180.0), 0.1, 100.0);
        var view = Matrix4.Identity();
        var transform = Matrix4.Translation(0.0, 0.0, -3.0);
        
        Material.Shader.SetUniform(0, projection * view);
        Material.Shader.SetUniform(1, transform);
        
        Mesh.Draw();
    }
}