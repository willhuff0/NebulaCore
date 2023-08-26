using Nebula.BuiltIn.Assets;
using Nebula.BuiltIn.Nodes;
using Nebula.Core;
using Nebula.Editor.Services;
using Nebula.Math;

var model = ModelService.DecodeModel(@"C:\Users\wehuf\Downloads\Avocado.glb", @"C:\Users\wehuf\RiderProjects\NebulaCore\TestProject\assets");
//File.WriteAllBytes(@"C:\Users\wehuf\RiderProjects\NebulaCore\TestProject\data\meshes\4fae96ea-cedd-446c-9fbb-3e397705ee52", model.);

Console.WriteLine(model);
return;

Engine.Init();

var project = new Project(@"C:\Users\wehuf\RiderProjects\NebulaCore\TestProject\Test Project.neb");

project.Database.LoadCollection("bc238247-94dd-4eef-979d-558ef4e555bd", true);

var nodeAsset = project.Database.GetLoadedAsset<NodeAsset>("af69467e-2677-45f4-8692-b7f92f8f3ad2")!;
var node = nodeAsset.Instantiate<RenderedNode>()!;

node.Material.Shader.Bind();

node.Material.Shader.SetUniform(2, new Vector3(0.0, 0.0, 0.0)); // WorldViewPos

node.Material.Shader.SetUniform(3, new Vector3(0.3, -0.8, 0.7).Normalized()); //DirectionalLightDirection
node.Material.Shader.SetUniform(4, new Vector3(1.0, 1.0, 1.0));
node.Material.Shader.SetUniform(5, 2.0);

//var node = new RenderedNode(null, "Test Node", mesh, material);

Engine.RootNode = node;

Engine.Loop();

Engine.Destroy();