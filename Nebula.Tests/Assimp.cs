// var assimp = new AssimpContext();
//
// var flags = PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals | PostProcessSteps.GenerateUVCoords | PostProcessSteps.OptimizeMeshes | PostProcessSteps.FlipUVs | PostProcessSteps.PreTransformVertices;
// var scene = assimp.ImportFile(@"C:\Users\wehuf\home\dev\NebulaCore\TestProject\test.glb", flags);
//
// var vertexData = scene.Meshes.SelectMany(mesh =>
// {
//     var data = new float[mesh.VertexCount * 8];
//     for (int i = 0; i < mesh.VertexCount; i++)
//     {
//         var j = i * 8;
//
//         var position = mesh.Vertices[i];
//         data[j] = position.X;
//         data[j + 1] = position.Y;
//         data[j + 2] = position.Z;
//
//         var normal = mesh.Normals[i];
//         data[j + 3] = normal.X;
//         data[j + 4] = normal.Y;
//         data[j + 5] = normal.Z;
//
//         var uv = mesh.TextureCoordinateChannels[0][i];
//         data[j + 6] = uv.X;
//         data[j + 7] = uv.Y;
//     }
//
//     return data;
// });
//
// var indexData = scene.Meshes.SelectMany(mesh => mesh.GetUnsignedIndices()).ToList();
// Console.WriteLine(indexData.Count);
//
// var vertexBytes = vertexData.SelectMany(BitConverter.GetBytes);
// var indexBytes = indexData.SelectMany(BitConverter.GetBytes);
//
// File.WriteAllBytes(@"C:\Users\wehuf\home\dev\NebulaCore\TestProject\test_opengl", indexBytes.Concat(vertexBytes).ToArray());
