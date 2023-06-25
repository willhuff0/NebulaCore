using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using Assimp;

namespace Nebula.Editor.Services;

public static class ModelService
{
    [UnmanagedCallersOnly(EntryPoint = "decodeMesh")]
    public static IntPtr DecodeModel(IntPtr inputPath, IntPtr outputDir)
    {
        var inputPathString = Marshal.PtrToStringUTF8(inputPath)!;
        var outputDirString = Marshal.PtrToStringUTF8(outputDir)!;

        var assimp = new AssimpContext();
        
        const PostProcessSteps flags = PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals | PostProcessSteps.GenerateUVCoords | PostProcessSteps.OptimizeMeshes | PostProcessSteps.FlipUVs | PostProcessSteps.PreTransformVertices;
        var scene = assimp.ImportFile(inputPathString, flags);

        var materials = scene.Materials.Select(material =>
        {
            var textures = new Dictionary<string, string>();

            if (material.GetMaterialTexture(TextureType.Diffuse, 0, out var albedo)) textures.Add("albedo", albedo.FilePath);
            if (material.GetMaterialTexture(TextureType.Metalness, 0, out var metallic)) textures.Add("metallic", metallic.FilePath);
            if (material.GetMaterialTexture(TextureType.Roughness, 0, out var roughness)) textures.Add("roughness", roughness.FilePath);
            if (material.GetMaterialTexture(TextureType.AmbientOcclusion, 0, out var occlusion)) textures.Add("occlusion", occlusion.FilePath);
            if (material.GetMaterialTexture(TextureType.Normals, 0, out var normal)) textures.Add("normal", normal.FilePath);

            return JsonValue.Create(textures);
        });

        var meshes = scene.Meshes.Select(mesh =>
        {
            var vertexData = new float[mesh.VertexCount * 8];
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                var j = i * 8;

                var position = mesh.Vertices[i];
                vertexData[j] = position.X;
                vertexData[j + 1] = position.Y;
                vertexData[j + 2] = position.Z;

                var normal = mesh.Normals[i];
                vertexData[j + 3] = normal.X;
                vertexData[j + 4] = normal.Y;
                vertexData[j + 5] = normal.Z;

                var uv = mesh.TextureCoordinateChannels[0][i];
                vertexData[j + 6] = uv.X;
                vertexData[j + 7] = uv.Y;
            }

            var indexData = mesh.GetUnsignedIndices();
            var material = mesh.MaterialIndex;

            return new JsonObject()
            {
                { "vertexData", JsonValue.Create(vertexData) },
                { "indexData", JsonValue.Create(indexData) },
                { "material", JsonValue.Create(material) },
            };
        });

        var result = new JsonObject()
        {
            { "materials", JsonValue.Create(materials) },
            { "meshes", JsonValue.Create(meshes) },
        };

        return Marshal.StringToCoTaskMemUTF8(result.ToJsonString());
    }
}