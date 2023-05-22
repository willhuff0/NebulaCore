using Nebula.BuiltIn.Assets;
using Nebula.BuiltIn.Nodes;
using Nebula.Core;

Engine.Init();

var project = new Project(@"C:\Users\wehuf\home\dev\NebulaCore\TestProject\Test Project.neb");

project.Database.LoadCollection("bc238247-94dd-4eef-979d-558ef4e555bd", true);

var mesh = project.Database.GetLoadedAsset<Mesh>("4fae96ea-cedd-446c-9fbb-3e397705ee52")!;
var material = project.Database.GetLoadedAsset<Material>("8398ebfd-66ed-4103-8ee3-f33faf8735a1")!;

var node = new RenderedNode(null, "Test Node", mesh, material);

Engine.RootNode = node;

Engine.Loop();

Engine.Destroy();