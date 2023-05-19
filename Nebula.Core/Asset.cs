using System.Text.Json.Nodes;

namespace Nebula.Core;

public class Project
{
    public readonly string AppDirectory;
    public readonly string UserDataDirectory;

    public readonly AssetDatabase Database;
}

public class AssetDatabase
{
    private static Dictionary<string, IAssetLoader> _loaders;

    public static void ReloadAssetLoaders()
    {
        
    }
    
    private Dictionary<Guid, AssetCollection> _collections;
    private Dictionary<Guid, AssetDefinition> _definitions;
    
    private Dictionary<Guid, Asset> _loadedAssets;

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

    protected AssetDefinition(string type, Guid key, JsonNode? properties)
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