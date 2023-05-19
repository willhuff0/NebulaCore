using Nebula.Graphics;

namespace Nebula.Core.Assets;

[AssetLoader("mesh")]
public class Mesh : Asset, IAssetLoader
{
    public static Task<Asset?> LoadAssetAsync(Project project, AssetDefinition definition)
    {
        try
        {
            var path = Path.Join(project.AppDirectory, "meshes", definition.Key.ToString());
            if (!File.Exists(path))
            {
                Console.WriteLine($"Error loading mesh asset ({definition.Key}): Data file at \"${path}\" does not exist.");
                return Task.FromResult<Asset?>(null);
            }

            var indexCount = definition.Properties?["indexCount"]?.GetValue<int?>();
            
            return Task.FromResult<Asset?>(new Mesh(definition.Type, definition.Key, path, indexCount));
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading mesh asset ({definition.Key}): {e}");
            return Task.FromResult<Asset?>(null);
        }
    }

    private readonly string _path;
    private readonly int? _indexCount;

    private Mesh(string type, Guid key, string path, int? indexCount) : base(type, key)
    {
        _path = path;
        _indexCount = indexCount;
    }

    private int _count;
    private uint _vao;
    private uint? _ebo;
    private uint _vbo;

    public override void EnterScope()
    {
        var data = File.ReadAllBytes(_path);

        const int bytesPerIndex = 2; // UInt16
        
        var indexData = _indexCount == null ? null : data.Take((int)_indexCount * bytesPerIndex).ToArray();
        var vertexData = _indexCount == null ? data : data.Skip((int)_indexCount * bytesPerIndex).ToArray();
        
        GL.GenVertexArrays(1, out var vaos);
        _vao = vaos[0];
        
        if (indexData != null)
        {
            GL.GenBuffers(1, out var ebos);
            _ebo = ebos[0];
            GL.BindBuffer(GL.ELEMENT_ARRAY_BUFFER, (uint)_ebo);
            GL.BufferData(GL.ELEMENT_ARRAY_BUFFER, indexData.LongLength, in indexData, GL.STATIC_DRAW);
        }

        GL.GenBuffers(1, out var vbos);
        _vbo = vbos[0];
        GL.BindBuffer(GL.ARRAY_BUFFER, _vbo);
        GL.BufferData(GL.ARRAY_BUFFER, vertexData.LongLength, in vertexData, GL.STATIC_DRAW);
        
        GL.EnableVertexAttribArray(0);
        GL.EnableVertexAttribArray(1);
        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(0, 3, GL.FLOAT, false, 8 * sizeof(float), 0 * sizeof(float));
        GL.VertexAttribPointer(1, 3, GL.FLOAT, false, 8 * sizeof(float), 3 * sizeof(float));
        GL.VertexAttribPointer(2, 2, GL.FLOAT, false, 8 * sizeof(float), 6 * sizeof(float));
        
        GL.BindVertexArray(0);

        _count = _indexCount ?? (int)(vertexData.LongLength / 32); /* 8 * sizeof(float) */
    }

    public override void ExitScope()
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

        if (_ebo != null) GL.DrawElements(GL.TRIANGLES, _count, GL.UNSIGNED_SHORT, 0);
        else GL.DrawArrays(GL.TRIANGLES, 0, _count);

        GL.BindVertexArray(0);
    }
}