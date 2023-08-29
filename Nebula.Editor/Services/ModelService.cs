using System.Numerics;
using glTFLoader;
using Nebula.Core;
using Nebula.Graphics;
using Vector3 = Nebula.Math.Vector3;

namespace Nebula.Editor.Services;

public static class ModelService
{
    public static void LoadGLTF(string path)
    {
        var gltf = Interface.LoadModel(path);
        
        var nodes = gltf.Nodes.Select((node) =>
        {
            Vector3 position;
            Quaternion rotation;
            Vector3 scale;

            if (node.Matrix is { Length: > 0 } && node.Matrix != new [] { 1F, 0F, 0F, 0F, 0F, 1F, 0F, 0F, 0F, 0F, 1F, 0F, 0F, 0F, 0F, 1F})
            {
                Matrix4x4.Decompose()
            }
        })
    }
}