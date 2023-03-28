using System.Text;
using System.Text.Json.Serialization;

namespace NebulaCore.engine;

public interface IAssetDefinition<TAsset, TRuntimeAsset>
{
    public static string name;
}

public abstract class Asset
{
    [JsonInclude] public string name;
    public Project project;

    private Task<RuntimeAsset?> load(RuntimeAssetBundle partialRuntimeAssetBundle);
}

public abstract class FileAsset : Asset
{
    [JsonInclude] public string path;

    public byte[] getData() => File.ReadAllBytes(Path.Join(project.root, path));

    public void setData(byte[] newData) => File.WriteAllBytes(Path.Join(project.root, path), newData);
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

    public byte[]? getData()
    {
        if (data != null) return data;
        if (path != null) return File.ReadAllBytes(Path.Join(project.root, path));
        return null;
    }

    /// <summary>
    /// If path is not null, sets data to disk only, otherwise sets data to memory only
    /// </summary>
    /// <param name="newData"></param>
    public void setData(byte[] newData)
    {
        if (path != null) File.WriteAllBytes(Path.Join(project.root, path), newData);
        else data = newData;
    }

    public void setDateToFile(byte[] newData, string? newPath = null)
    {
        if (newPath != null) path = newPath;
        if (path == null) return;
        File.WriteAllBytes(Path.Join(project.root, path), newData);
    }

    /// <summary>
    /// Writes memory to file then deletes memory
    /// </summary>
    public void moveToFile()
    {
        if (data != null) File.WriteAllBytes(Path.Join(project.root, path), data);
        data = null;
    }

    /// <summary>
    /// Reads file to memory then deletes file
    /// </summary>
    public void moveToMemory()
    {
        if (path != null)
        {
            var absPath = Path.Join(project.root, path);
            data = File.ReadAllBytes(absPath);
            File.Delete(absPath);
        }
    }
}

public abstract class RuntimeAsset
{
    public abstract void unload();
}