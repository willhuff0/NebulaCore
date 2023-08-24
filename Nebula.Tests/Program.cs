using Nebula.BuiltIn.Assets;
using Nebula.BuiltIn.Nodes;
using Nebula.Core;
using Nebula.Math;

Engine.Init();

var project = new Project(@"C:\Users\wehuf\RiderProjects\NebulaCore\TestProject\Test Project.neb");

project.Database.LoadCollection("bc238247-94dd-4eef-979d-558ef4e555bd", true);

var mesh = project.Database.GetLoadedAsset<Mesh>("4fae96ea-cedd-446c-9fbb-3e397705ee52")!;
var material = project.Database.GetLoadedAsset<Material>("8398ebfd-66ed-4103-8ee3-f33faf8735a1")!;

material.Shader.Bind();

material.Shader.SetUniform(2, new Vector3(0.0, 0.0, 0.0)); // WorldViewPos

material.Shader.SetUniform(3, new Vector3(0.3, -0.8, 0.7).Normalized()); //DirectionalLightDirection
material.Shader.SetUniform(4, new Vector3(1.0, 1.0, 1.0));
material.Shader.SetUniform(5, 2.0);

var node = new RenderedNode(null, "Test Node", mesh, material);

Engine.RootNode = node;

Engine.Loop();

Engine.Destroy();