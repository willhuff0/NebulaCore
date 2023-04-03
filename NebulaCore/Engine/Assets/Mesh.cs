using System.Text.Json.Nodes;
using NebulaCore.Angle;

namespace NebulaCore.Engine.Assets;

[AssetDefinition("meshes")]
public class Mesh : FileAsset
{
    private int? _indexCount;
    
    public Mesh(Project project, JsonNode json) : base(project, json)
    {
        _indexCount = json["count"]?.GetValue<int>();
    }

    public override JsonObject Serialize()
    {
        var json = base.Serialize();
        if (_indexCount != null) json["indexCount"] = JsonValue.Create(_indexCount);
        return json;
    }

    public override Task<RuntimeAsset?> Load()
    {
        var data = FileAssetGetBytes();
        
        const int bytesPerIndex = 2; // UInt16
        
        dynamic? indexData = _indexCount == null ? null : data.Take((int)_indexCount * bytesPerIndex).ToArray();
        dynamic? vertexData = _indexCount == null ? data : data.Skip((int)_indexCount * bytesPerIndex).ToArray();

        GL.GenVertexArrays(1, out var vaos);
        var vao = vaos[0];

        uint? ebo = null;
        if (indexData != null)
        {
            GL.GenBuffers(1, out var ebos);
            ebo = ebos[0];
            GL.BindBuffer(GL.ELEMENT_ARRAY_BUFFER, (uint)ebo);
            GL.BufferData(GL.ELEMENT_ARRAY_BUFFER, indexData.LongLength, in indexData, GL.STATIC_DRAW);
        }

        GL.GenBuffers(1, out var vbos);
        var vbo = vbos[0];
        GL.BindBuffer(GL.ARRAY_BUFFER, vbo);
        GL.BufferData(GL.ARRAY_BUFFER, vertexData.LongLength, in vertexData, GL.STATIC_DRAW);
        
        GL.EnableVertexAttribArray(0);
        GL.EnableVertexAttribArray(1);
        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(0, 3, GL.FLOAT, false, 8 * sizeof(float), 0 * sizeof(float));
        GL.VertexAttribPointer(1, 3, GL.FLOAT, false, 8 * sizeof(float), 3 * sizeof(float));
        GL.VertexAttribPointer(2, 2, GL.FLOAT, false, 8 * sizeof(float), 6 * sizeof(float));
        
        GL.BindVertexArray(0);

        return Task.FromResult<RuntimeAsset?>(new RuntimeMesh(Project, _indexCount != null ? (int)_indexCount : (int)((long)vertexData.LongLength / 32 /* 8 * sizeof(float) */), vao, ebo, vbo));
    }
}

public class RuntimeMesh : RuntimeAsset
{
    private int _count;
    private uint _vao;
    private uint? _ebo;
    private uint _vbo;
    
    public RuntimeMesh(Project project, int count, uint vao, uint? ebo, uint vbo) : base(project)
    {
        _count = count;
        _vao = vao;
        _ebo = ebo;
        _vbo = vbo;
    }

    public override Task Unload()
    {
        GL.DeleteVertexArrays(1, new []{_vao});
        if(_ebo != null)  GL.DeleteBuffers(1, new []{(uint)_ebo});
        GL.DeleteBuffers(1, new []{_vbo});
        return Task.CompletedTask;
    }

    public void Draw()
    {
        GL.BindVertexArray(_vao);

        if (_ebo != null) GL.DrawElements(GL.TRIANGLES, _count, GL.UNSIGNED_SHORT, 0);
        else GL.DrawArrays(GL.TRIANGLES, 0, _count);

        GL.BindVertexArray(0);
    }
}