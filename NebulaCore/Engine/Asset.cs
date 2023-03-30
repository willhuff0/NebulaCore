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
    public string name;

    protected Asset(Project project, JsonNode json)
    {
        Project = project;
        name = json["name"].GetValue<string>();
    }
    
    public abstract JsonObject Serialize();
    
    public abstract Task<RuntimeAsset?> Load();
}

public abstract class RuntimeAsset
{
    protected Project Project;
    
    protected RuntimeAsset(Project project)
    {
        Project = project;
    }
    
    public virtual Task AssignRuntimeReferences() => Task.CompletedTask;
    
    public abstract Task Unload();
}

public abstract class FileAsset : Asset
{
    protected string path;

    protected FileAsset(Project project, JsonNode json) : base(project, json)
    {
        path = json["path"].GetValue<string>();
    }

    public override JsonObject Serialize() => new JsonObject()
    {
        { "path", path }
    };

    protected byte[] GetData() => File.ReadAllBytes(Path.Join(Project.Root, path));

    protected void SetData(byte[] newData) => File.WriteAllBytes(Path.Join(Project.Root, path), newData);

    protected string GetDataAsString() => File.ReadAllText(Path.Join(Project.Root, path));

    protected void SetDataAsString(string newData) => File.WriteAllText(Path.Join(Project.Root, path), newData);
}

public abstract class MemoryAsset : Asset
{
    protected byte[] data;
    
    protected MemoryAsset(Project project, JsonNode json) : base(project, json)
    {
        data = Convert.FromBase64String(json["data"].GetValue<string>());
    }

    public override JsonObject Serialize() => new JsonObject()
    {
        { "data", Convert.ToBase64String(data) }
    };

    protected string GetDataAsString() => Encoding.UTF8.GetString(data);

    protected void SetDataAsString(string newData) => Encoding.UTF8.GetBytes(newData);
}

public abstract class FileOrMemoryAsset : Asset
{
    protected byte[]? data;
    protected string? path;

    protected FileOrMemoryAsset(Project project, JsonNode json) : base(project, json)
    {
        var _data = json["data"]?.GetValue<string>();
        if (_data != null) data = Convert.FromBase64String(_data);

        path = json["path"]?.GetValue<string>();
    }

    public override JsonObject Serialize()
    {
        var json = new JsonObject();
        if (data != null) json["data"] = JsonValue.Create(data);
        if (path != null) json["path"] = JsonValue.Create(path);
        return json;
    }

    protected byte[]? GetData()
    {
        if (data != null) return data;
        if (path != null) return File.ReadAllBytes(Path.Join(Project.Root, path));
        return null;
    }

    /// <summary>
    /// If path is not null, sets data to disk only, otherwise sets data to memory only
    /// </summary>
    /// <param name="newData"></param>
    protected void SetData(byte[] newData)
    {
        if (path != null) File.WriteAllBytes(Path.Join(Project.Root, path), newData);
        else data = newData;
    }

    protected void SetFile(byte[] newData, string? newPath = null)
    {
        if (newPath != null) path = newPath;
        if (path == null) return;
        File.WriteAllBytes(Path.Join(Project.Root, path), newData);
    }

    /// <summary>
    /// Writes memory to file then deletes memory
    /// </summary>
    protected void MoveToFile()
    {
        if (data != null) File.WriteAllBytes(Path.Join(Project.Root, path), data);
        data = null;
    }

    /// <summary>
    /// Reads file to memory then deletes file
    /// </summary>
    protected void MoveToMemory()
    {
        if (path != null)
        {
            var absPath = Path.Join(Project.Root, path);
            data = File.ReadAllBytes(absPath);
            File.Delete(absPath);
        }
    }
}