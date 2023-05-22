using Nebula.Core;
using Nebula.Graphics;

namespace Nebula.BuiltIn.Assets;

[AssetLoader("mesh")]
public class Mesh : Asset, IAssetLoader
{
    public static Asset? LoadAsset(Project project, AssetDefinition definition, bool immediateEnterScope)
    {
        try
        {
            var path = Path.Join(project.AppDirectory, "meshes", definition.Key.ToString());
            if (!File.Exists(path))
            {
                Console.WriteLine($"Error loading mesh asset ({definition.Key}): Data file at \"${path}\" does not exist.");
                return null;
            }

            var indexCount = definition.Properties?["indexCount"]?.GetValue<int?>();
            
            var mesh = new Mesh(project.Database, definition.Type, definition.Key, path, indexCount);
            if (immediateEnterScope) mesh.EnterScope();
            return mesh;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading mesh asset ({definition.Key}): {e}");
            return null;
        }
    }

    private readonly string _path;
    private readonly int? _indexCount;

    private Mesh(AssetDatabase database, string type, Guid key, string path, int? indexCount) : base(database, type, key)
    {
        _path = path;
        _indexCount = indexCount;
    }

    private int _count;
    private uint _vao;
    private uint? _ebo;
    private uint _vbo;

    protected override void EnterScope()
    {
        var data = File.ReadAllBytes(_path);

        const int bytesPerIndex = 4; // UInt32
        
        var indexData = _indexCount == null ? null : data.Take((int)_indexCount * bytesPerIndex).ToArray();
        var vertexData = _indexCount == null ? data : data.Skip((int)_indexCount * bytesPerIndex).ToArray();
        
        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);
        
        if (indexData != null)
        {
            _ebo = GL.GenBuffer();
            GL.BindBuffer(GL.ELEMENT_ARRAY_BUFFER, (uint)_ebo);
            GL.BufferData(GL.ELEMENT_ARRAY_BUFFER, indexData.LongLength, indexData, GL.STATIC_DRAW);
        }
        
        _vbo = GL.GenBuffer();
        GL.BindBuffer(GL.ARRAY_BUFFER, _vbo);
        GL.BufferData(GL.ARRAY_BUFFER, vertexData.LongLength, vertexData, GL.STATIC_DRAW);
        
        GL.EnableVertexAttribArray(0);
        GL.EnableVertexAttribArray(1);
        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(0, 3, GL.FLOAT, false, 8 * sizeof(float), 0 * sizeof(float)); // Positions
        GL.VertexAttribPointer(1, 3, GL.FLOAT, false, 8 * sizeof(float), 3 * sizeof(float)); // Normals
        GL.VertexAttribPointer(2, 2, GL.FLOAT, false, 8 * sizeof(float), 6 * sizeof(float)); // UVs
        
        GL.BindVertexArray(0);

        _count = _indexCount ?? (int)(vertexData.LongLength / 32); /* 8 * sizeof(float) */
    }

    protected override void ExitScope()
    {
        _count = 0;
        
        GL.DeleteVertexArrays(1, new []{_vao});
        _vao = 0;
        
        if(_ebo != null)  GL.DeleteBuffers(1, new []{(uint)_ebo});
        _ebo = null;
        
        GL.DeleteBuffers(1, new []{_vbo});
        _vbo = 0;
    }
    
    public void Draw()
    {
        GL.BindVertexArray(_vao);

        if (_ebo != null) GL.DrawElements(GL.TRIANGLES, _count, GL.UNSIGNED_INT, 0);
        else GL.DrawArrays(GL.TRIANGLES, 0, _count);

        GL.BindVertexArray(0);
    }
}