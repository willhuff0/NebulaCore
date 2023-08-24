using System.Text.Json.Nodes;
using Nebula.Core;
using Nebula.BuiltIn.Assets;
using Nebula.Math;

namespace Nebula.BuiltIn.Nodes;

[NodeLoader("rendered")]
public class RenderedNode : Node, INodeLoader
{
    public static Node? LoadNode(Project project, string name, JsonNode? definition)
    {
        var mesh = project.Database.GetLoadedAsset<Mesh>(definition!["mesh"]!.GetValue<string>())!;
        var material = project.Database.GetLoadedAsset<Material>(definition!["material"]!.GetValue<string>())!;

        return new RenderedNode(null, name, mesh, material);
    }
    
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
        
        var projection = Matrix4.Perspective(args.FrameArgs.Aspect, 25.0 * (double.Pi / 180.0), 0.1, 100.0);
        var view = Matrix4.Translation(0.0, 0.0, -10.0);

        Material.Shader.SetUniform(0, projection * view); // ViewProjectionMatrix
        
        var transform = Matrix4.YRotation(args.FrameArgs.TimeTotal * (45.0 * (double.Pi / 180.0)));

        Material.Shader.SetUniform(1, transform); // TransformMatrix
        
        Mesh.Draw();
    }
}