using System.Reflection;
using System.Text.Json.Nodes;

namespace Nebula.Core;

public class Project
{
    public string Name { get; private set; }
    
    public readonly string RootDirectory;
    public readonly string AppDirectory;
    public readonly string UserDataDirectory;

    public readonly AssetDatabase Database;

    private Project(string path, string name, AssetDatabase database)
    {
        RootDirectory = Path.GetDirectoryName(path)!;
        AppDirectory = Path.Join(RootDirectory, "Data");
        UserDataDirectory = Path.Join(RootDirectory, "SaveGame");
        Name = name;
        Database = database;
    }
    
    public Project(string path, string name)
    {
        RootDirectory = Path.GetDirectoryName(path)!;
        AppDirectory = Path.Join(RootDirectory, "Data");
        UserDataDirectory = Path.Join(RootDirectory, "SaveGame");
        Name = name;

        Database = new AssetDatabase();
        
        if (Directory.EnumerateFileSystemEntries(RootDirectory).Any()) throw new Exception($"Failed to create project: Directory must be empty ({RootDirectory})");
        File.WriteAllText(path, new JsonObject() { {"name", name} }.ToJsonString());
    }

    public static Project Load(string path)
    {
        var content = File.ReadAllText(path);
        var json = JsonNode.Parse(content);

        var name = json!["name"]!.GetValue<string>();
        var database = new AssetDatabase(json.AsObject());

        return new Project(path, name, database);
    }
}

public class AssetDatabase
{
    private static Dictionary<string, LoadAssetAsyncDelegate> _loaders = new();

    public static void ReloadAssetLoaders()
    {
        var loaders =
            from type in Assembly.GetCallingAssembly().GetTypes().AsParallel()
            let attributes = type.GetCustomAttributes(typeof(AssetLoaderAttribute), false)
            where attributes is { Length: > 0 }
            select new { Type = type, Attributes = attributes.Cast<AssetLoaderAttribute>() };

        foreach (var loader in loaders)
        {
            _loaders[loader.Attributes.First().For] = loader.Type.GetMethod("LoadAssetAsync")!.CreateDelegate<LoadAssetAsyncDelegate>();
        }
    }
    
    private Dictionary<Guid, AssetCollection> _collections;
    private Dictionary<Guid, AssetDefinition> _definitions;
    
    private Dictionary<Guid, Asset> _loadedAssets;

    public AssetDatabase()
    {
        ReloadAssetLoaders();

        _collections = new Dictionary<Guid, AssetCollection>();
        _definitions = new Dictionary<Guid, AssetDefinition>();

        _loadedAssets = new Dictionary<Guid, Asset>();
    }

    public AssetDatabase(JsonObject json) : this()
    {
        ImportCollections(json["collections"]!.AsObject());
        ImportDefinitions(json["definitions"]!.AsObject());
    }
    
    public void ImportCollections(JsonObject json)
    {
        foreach (var (key, assetKeys) in json)
        {
            var guid = Guid.Parse(key);
            var assetGuids = assetKeys!.AsArray().Select(assetKey => Guid.Parse(assetKey!.GetValue<string>())).ToArray();
            _collections.Add(guid, new AssetCollection(guid, assetGuids));
        }
    }
    
    public void ImportDefinitions(JsonObject json)
    {
        foreach (var (type, group) in json)
        {
            foreach (var (key, properties) in group!.AsObject())
            {
                var guid = Guid.Parse(key);
                _definitions.Add(guid, new AssetDefinition(type, guid, properties));
            }
        }
    }

    public async Task LoadCollection(string key)
    {
        var 
    }

    public T? GetAsset<T>(Guid key) where T : Asset => _loadedAssets[key] as T;
}

[AttributeUsage(AttributeTargets.Class)]
public class AssetLoaderAttribute : Attribute
{
    public readonly string For;

    public AssetLoaderAttribute(string @for)
    {
        For = @for;
    }
}

internal delegate Task<Asset?> LoadAssetAsyncDelegate(Project project, AssetDefinition definition);

public interface IAssetLoader
{
    public static virtual Task<Asset?> LoadAssetAsync(Project project, AssetDefinition definition) => Task.FromResult<Asset?>(null);
}

public abstract class Asset
{
    public readonly string Type;
    public readonly Guid Key;

    protected Asset(string type, Guid key)
    {
        Type = type;
        Key = key;
    }

    public virtual void LookupReferences(AssetDatabase database)
    {
        
    }

    public abstract void EnterScope();
    
    public abstract void ExitScope();
}

public class AssetDefinition
{
    public readonly string Type;
    public readonly Guid Key;
    public readonly JsonNode? Properties;

    internal AssetDefinition(string type, Guid key, JsonNode? properties)
    {
        Type = type;
        Key = key;
        Properties = properties;
    }
}

public class AssetCollection
{
    public readonly Guid Key;
    public readonly Guid[] Assets;

    public AssetCollection(Guid key, Guid[] assets)
    {
        Key = key;
        Assets = assets;
    }
}