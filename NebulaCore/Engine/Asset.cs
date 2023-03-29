using System.Text;
using System.Text.Json.Serialization;

namespace NebulaCore.Engine;

public interface IAssetDefinitions
{
    public static virtual (Type, Type) GetDefinition(string name)
    {
        throw new NotImplementedException();
    }
}

public class DefaultAssetDefinitions : IAssetDefinitions {
    public static Dictionary<string, Type> GetDefinitions()
    {
        return new Dictionary<string, Type>()
        {
            //{ "shaders", typeof(TestShader) }
        };
    }
}

public interface IAsset<TRuntimeAsset>
where TRuntimeAsset : IRuntimeAsset
{
    public Task<TRuntimeAsset?> Load();
}

public interface IRuntimeAsset
{
    public Task Unload();
    public Task AssignRuntimeReferences();
}

public abstract class Asset : IAsset<IRuntimeAsset>
{
    [JsonInclude] public string name;
    public Project project;
    public abstract Task<IRuntimeAsset?> Load();
}

public abstract class RuntimeAsset : IRuntimeAsset
{
    public abstract Task Unload();
    public abstract Task AssignRuntimeReferences();
}

public abstract class FileAsset : Asset
{
    [JsonInclude] public string path;

    public byte[] GetData() => File.ReadAllBytes(Path.Join(project.Root, path));

    public void SetData(byte[] newData) => File.WriteAllBytes(Path.Join(project.Root, path), newData);
}

public abstract class DataAsset : Asset
{
    public byte[] data;

    [JsonInclude]
    public string base64
    {
        get => Encoding.UTF8.GetString(data);
        set => data = Encoding.UTF8.GetBytes(value);
    }
}

public abstract class FileOrMemoryAsset : Asset
{
    public byte[]? data;
    [JsonInclude] public string? path;

    [JsonInclude]
    public string? base64
    {
        get => data == null ? null : Encoding.UTF8.GetString(data);
        set => data = value == null ? null : Encoding.UTF8.GetBytes(value);
    }

    public byte[]? GetData()
    {
        if (data != null) return data;
        if (path != null) return File.ReadAllBytes(Path.Join(project.Root, path));
        return null;
    }

    /// <summary>
    /// If path is not null, sets data to disk only, otherwise sets data to memory only
    /// </summary>
    /// <param name="newData"></param>
    public void SetData(byte[] newData)
    {
        if (path != null) File.WriteAllBytes(Path.Join(project.Root, path), newData);
        else data = newData;
    }

    public void SetFile(byte[] newData, string? newPath = null)
    {
        if (newPath != null) path = newPath;
        if (path == null) return;
        File.WriteAllBytes(Path.Join(project.Root, path), newData);
    }

    /// <summary>
    /// Writes memory to file then deletes memory
    /// </summary>
    public void MoveToFile()
    {
        if (data != null) File.WriteAllBytes(Path.Join(project.Root, path), data);
        data = null;
    }

    /// <summary>
    /// Reads file to memory then deletes file
    /// </summary>
    public void MoveToMemory()
    {
        if (path != null)
        {
            var absPath = Path.Join(project.Root, path);
            data = File.ReadAllBytes(absPath);
            File.Delete(absPath);
        }
    }
}