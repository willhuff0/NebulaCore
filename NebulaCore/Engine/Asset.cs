using System.Text;
using System.Text.Json.Nodes;

namespace NebulaCore.Engine;

[AttributeUsage(AttributeTargets.Class)]
public class AssetDefinitionAttribute : Attribute
{
    private string _groupName;

    public AssetDefinitionAttribute(string groupName)
    {
        _groupName = groupName;
    }

    public string GroupName => _groupName;
}

public abstract class Asset
{
    protected Project Project;
    public string Name;
    protected internal List<Guid> AssociatedCollections;

    protected Asset(Project project, JsonNode json)
    {
        Project = project;
        Name = json["name"]!.GetValue<string>();
        AssociatedCollections = json["collections"]?.GetValue<List<Guid>>() ?? new List<Guid>();
    }

    public bool IsAssociated(Guid collection) => AssociatedCollections.Contains(collection);
    public void AddAssociation(Guid collection) => AssociatedCollections.Add(collection);
    public void RemoveAssociation(Guid collection) => AssociatedCollections.Remove(collection);
    
    public abstract JsonObject Serialize();

    public abstract Task<RuntimeAsset?> Load();
}

public abstract class RuntimeAsset
{
    protected Project Project;
    private List<Guid> _associatedCollections;
    
    protected RuntimeAsset(Project project, Asset from)
    {
        Project = project;
        _associatedCollections = from.AssociatedCollections;
    }
    
    public bool IsAssociated(Guid collection) => _associatedCollections.Contains(collection);
    public void AddAssociation(Guid collection) => _associatedCollections.Add(collection);
    public void RemoveAssociation(Guid collection) => _associatedCollections.Remove(collection);
    
    public virtual Task AssignRuntimeReferences() => Task.CompletedTask;
    
    public abstract Task Unload();
}

public abstract class FileAsset : Asset
{
    private string _path;

    protected FileAsset(Project project, JsonNode json) : base(project, json)
    {
        _path = json["path"]!.GetValue<string>();
    }

    public override JsonObject Serialize() => new JsonObject()
    {
        { "path", _path }
    };

    protected string AbsolutePath => Path.Join(Project.Root, _path);

    protected FileStream FileAssetOpenRead() => File.OpenRead(AbsolutePath);

    protected byte[] FileAssetGetBytes() => File.ReadAllBytes(AbsolutePath);
    protected void FileAssetSetBytes(byte[] newData) => File.WriteAllBytes(AbsolutePath, newData);

    protected string FileAssetGetText() => File.ReadAllText(AbsolutePath);
    protected void FileAssetSetText(string newData) => File.WriteAllText(AbsolutePath, newData);
}

public abstract class MemoryAsset : Asset
{
    private byte[] _data;
    
    protected MemoryAsset(Project project, JsonNode json) : base(project, json)
    {
        _data = Convert.FromBase64String(json["data"]!.GetValue<string>());
    }

    public override JsonObject Serialize() => new JsonObject()
    {
        { "data", Convert.ToBase64String(_data) }
    };

    protected byte[] MemoryAssetGetBytes() => _data;
    protected void MemoryAssetSetBytes(byte[] newData) => _data = newData;

    protected string MemoryAssetGetText() => Encoding.UTF8.GetString(_data);
    protected void MemoryAssetSetText(string newData) => Encoding.UTF8.GetBytes(newData);
}

public abstract class FileOrMemoryAsset : Asset
{
    private byte[]? _data;
    private string? _path;

    protected FileOrMemoryAsset(Project project, JsonNode json) : base(project, json)
    {
        var dataJson = json["data"]?.GetValue<string>();
        if (dataJson != null) _data = Convert.FromBase64String(dataJson);

        _path = json["path"]?.GetValue<string>();
    }

    public override JsonObject Serialize()
    {
        var json = new JsonObject();
        if (_data != null) json["data"] = JsonValue.Create(_data);
        if (_path != null) json["path"] = JsonValue.Create(_path);
        return json;
    }
    
    protected string? AbsolutePath => _path == null ? null : Path.Join(Project.Root, _path);

    protected byte[]? AssetGetBytes()
    {
        if (_data != null) return _data;
        if (_path != null) return File.ReadAllBytes(AbsolutePath!);
        return null;
    }

    /// <summary>
    /// If path is not null, sets data to disk only, otherwise sets data to memory only
    /// </summary>
    /// <param name="newData"></param>
    protected void AssetSetBytes(byte[] newData)
    {
        if (_path != null) File.WriteAllBytes(AbsolutePath!, newData);
        else _data = newData;
    }

    protected void AssetSetFile(byte[] newData, string? newPath = null)
    {
        if (newPath != null)
        {
            if (_path != null) File.Delete(AbsolutePath!);
            _path = newPath;
        }
        if (_path == null) return;
        File.WriteAllBytes(AbsolutePath!, newData);
    }

    /// <summary>
    /// Writes memory to file then deletes memory. Does nothing if path and newPath are null.
    /// </summary>
    protected void AssetMoveToFile(string? newPath = null)
    {
        if (newPath != null)
        {
            if (_path != null) File.Delete(AbsolutePath!);
            _path = newPath;
        }
        if (_path == null) return;
        if (_data != null) File.WriteAllBytes(AbsolutePath!, _data);
        _data = null;
    }

    /// <summary>
    /// Reads file to memory then deletes file
    /// </summary>
    protected void AssetMoveToMemory()
    {
        if (_path != null)
        {
            var absPath = AbsolutePath!;
            _data = File.ReadAllBytes(absPath);
            File.Delete(absPath);
        }
    }
}