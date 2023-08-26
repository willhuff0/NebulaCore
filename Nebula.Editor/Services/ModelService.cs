using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using Assimp;
using SixLabors.ImageSharp.Formats.Webp;

namespace Nebula.Editor.Services;

public static class ModelService
{
    [UnmanagedCallersOnly(EntryPoint = "decodeMesh")]
    public static IntPtr DecodeModel(IntPtr inputPath, IntPtr outputDir)
    {
        var inputPathString = Marshal.PtrToStringUTF8(inputPath)!;
        var outputDirString = Marshal.PtrToStringUTF8(outputDir)!;

        var result = DecodeModel(inputPathString, outputDirString);

        return Marshal.StringToCoTaskMemUTF8(result.ToJsonString());
    }
    
    public static JsonObject DecodeModel(string inputPath, string outputDir)
    {
        var assimp = new AssimpContext();
        
        const PostProcessSteps flags = PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals | PostProcessSteps.GenerateUVCoords | PostProcessSteps.OptimizeMeshes | PostProcessSteps.FlipUVs | PostProcessSteps.PreTransformVertices;
        var scene = assimp.ImportFile(inputPath, flags);
        
        string AddTexture(string type, TextureSlot slot)
        {
            var texture = scene.GetEmbeddedTexture(slot.FilePath);
            Image image;
            if (texture.IsCompressed)
            {
                image = Image.Load(texture.CompressedData);
            }
            else
            {
                var rgba32Data = texture.NonCompressedData.Select(texel => new Rgba32(texel.R, texel.G, texel.B, texel.A)).ToArray();
                image = Image.LoadPixelData<Rgba32>(rgba32Data, texture.Width, texture.Height);
            }

            var dir = Path.Join(outputDir, "textures");
            var path = Path.Join(dir, $"{type}.webp");
            Directory.CreateDirectory(dir);
            image.Save(path, new WebpEncoder()
            {
                FileFormat = WebpFileFormatType.Lossless,
            });

            return path;
            
            // https://github.com/ARM-software/astc-encoder
        }

        var materials = scene.Materials.Select(material =>
        {
            var textures = new Dictionary<string, string>();

            if (material.GetMaterialTexture(TextureType.Diffuse, 0, out var albedo)) textures.Add("albedo", AddTexture("albedo", albedo));
            if (material.GetMaterialTexture(TextureType.Metalness, 0, out var metallic)) textures.Add("metallic", AddTexture("metallic", metallic));
            if (material.GetMaterialTexture(TextureType.Roughness, 0, out var roughness)) textures.Add("roughness", AddTexture("roughness", roughness));
            if (material.GetMaterialTexture(TextureType.AmbientOcclusion, 0, out var occlusion)) textures.Add("occlusion", AddTexture("occlusion", occlusion));
            if (material.GetMaterialTexture(TextureType.Normals, 0, out var normal)) textures.Add("normal", AddTexture("normal", normal));

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

        return result;
    }
}