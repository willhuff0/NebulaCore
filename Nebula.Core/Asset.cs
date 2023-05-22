using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json.Nodes;
using Nebula.Graphics;

namespace Nebula.Core;

public class AssetDatabase
{
    private static readonly Dictionary<string, LoadAssetDelegate> _loaders = new();

    private static void ReloadAssetLoaders()
    {
        var loaders =
            from assembly in AppDomain.CurrentDomain.GetAssemblies().AsParallel()
            from type in assembly.GetTypes()
            let attributes = type.GetCustomAttributes(typeof(AssetLoaderAttribute), false)
            where attributes is { Length: > 0 }
            select new { Type = type, Attributes = attributes.Cast<AssetLoaderAttribute>() };

        foreach (var loader in loaders)
        {
            _loaders[loader.Attributes.First().For] = loader.Type.GetMethod("LoadAsset")!.CreateDelegate<LoadAssetDelegate>();
        }
    }

    public readonly Project AssociatedProject;

    private readonly Dictionary<Guid, AssetCollection> _collections;
    private readonly Dictionary<Guid, AssetDefinition> _definitions;
    
    private readonly Dictionary<Guid, Asset> _loadedAssets;

    public AssetDatabase(Project project)
    {
        ReloadAssetLoaders();
        
        AssociatedProject = project;

        _collections = new Dictionary<Guid, AssetCollection>();
        _definitions = new Dictionary<Guid, AssetDefinition>();

        _loadedAssets = new Dictionary<Guid, Asset>();
    }

    public AssetDatabase(Project project, JsonObject json) : this(project)
    {
        ImportCollections(json["collections"]!.AsObject());
        ImportDefinitions(json["definitions"]!.AsObject());
    }
    
    public void ImportCollections(JsonObject json)
    {
        foreach (var (key, assetKeys) in json)
        {
            var guid = Guid.Parse(key);
            var assetGuids = assetKeys!.AsArray().Select(assetKey => Guid.Parse(assetKey!.GetValue<string>())).ToImmutableHashSet();
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

    public void LoadCollection(string key, bool immediateEnterScope = false) => LoadCollection(Guid.Parse(key), immediateEnterScope);
    public void LoadCollection(Guid key, bool immediateEnterScope = false)
    {
        var collection = _collections[key];
        var collectionLoadedAssets = collection.Assets.Aggregate(new Dictionary<Guid, Asset>(), (list, assetKey) =>
        {
            if (_loadedAssets.ContainsKey(assetKey)) return list;

            var definition = _definitions[assetKey];
            var loader = _loaders[definition.Type];

            var asset = loader(AssociatedProject, definition, immediateEnterScope);
#if DEBUG
            var error = GL.GetError();
            if (error != GL.NO_ERROR)
            {
                Console.WriteLine($"Error while loading asset {definition.Key}, OpenGL ES error {error}, {GL.GetErrorDescription(error)}");
            }
#endif
            if (asset != null)
            {
                list.Add(assetKey, asset);
                _loadedAssets.Add(assetKey, asset);
            }

            return list;
        });
        
        foreach (var (assetKey, asset) in collectionLoadedAssets)
        {
            asset.LookupReferences();
            if (immediateEnterScope) asset._immediateEnterScope();
        }
    }

    public void UnloadCollection(string key) => UnloadCollection(Guid.Parse(key));
    public void UnloadCollection(Guid key)
    {
        var collection = _collections[key];
        foreach (var assetKey in collection.Assets)
        {
            if (_loadedAssets.Remove(assetKey, out var asset))
            {
                asset._destroy();
            }
        }
    }

    public void UnloadAll()
    {
        foreach (var (assetKey, asset) in _loadedAssets)
        {
            asset._destroy();
        }
        _loadedAssets.Clear();
    }

    public T? GetLoadedAsset<T>(string key) where T : Asset => GetLoadedAsset<T>(Guid.Parse(key));
    public T? GetLoadedAsset<T>(Guid key) where T : Asset => _loadedAssets[key] as T;
    public Asset? GetLoadedAsset(string key) => GetLoadedAsset(Guid.Parse(key));
    public Asset? GetLoadedAsset(Guid key) => _loadedAssets[key];

    public override string ToString() => $"Asset Database for {AssociatedProject.Name}:\n" +
                                         $"  Loaders: [{string.Join(", ", _loaders.Select(pair => $"{pair.Key}"))}]\n" +
                                         $"  Definitions: [{string.Join(", ", _definitions.Select(pair => pair.Value))}]\n" +
                                         $"  Loaded Assets: [{string.Join(", ", _loadedAssets.Select(pair => pair.Value))}]";
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

internal delegate Asset? LoadAssetDelegate(Project project, AssetDefinition definition, bool immediateEnterScope);

public interface IAssetLoader
{
    public static abstract Asset? LoadAsset(Project project, AssetDefinition definition, bool immediateEnterScope);
}

public abstract class Asset : IEquatable<Asset>
{
    public readonly AssetDatabase Database;
    
    public readonly string Type;
    public readonly Guid Key;

    private bool _inScope;

    protected Asset(AssetDatabase database, string type, Guid key)
    {
        Database = database;
        Type = type;
        Key = key;
    }

    public virtual void LookupReferences() { }

    public HashSet<Asset> Dependencies => GatherDependencies().Append(this).ToHashSet();

    protected virtual HashSet<Asset> GatherDependencies() => new();

    internal void _immediateEnterScope()
    {
        _inScope = true;
    }
    
    internal void _enterScope()
    {
        _inScope = true;
        EnterScope();
    }
    
    internal void _exitScope()
    {
        _inScope = false;
        ExitScope();
    }

    internal void _destroy()
    {
        if (_inScope) _exitScope();
        Destroy();
    }

    protected virtual void EnterScope() { }

    protected virtual void ExitScope() { }

    protected virtual void Destroy() { }

    public bool Equals(Asset? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Key.Equals(other.Key);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Asset)obj);
    }

    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }

    public override string ToString() => $"{Type} Asset ({Key}, in scope: {_inScope})";
}

public class AssetDefinition : IEquatable<AssetDefinition>
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

    public bool Equals(AssetDefinition? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Key.Equals(other.Key);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((AssetDefinition)obj);
    }

    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }

    public override string ToString() => $"{Type} Asset Definition ({Key})";
}

public class AssetCollection : IEquatable<AssetCollection>
{
    public readonly Guid Key;
    public readonly ImmutableHashSet<Guid> Assets;

    public AssetCollection(Guid key, ImmutableHashSet<Guid> assets)
    {
        Key = key;
        Assets = assets;
    }

    public bool Equals(AssetCollection? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Key.Equals(other.Key);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((AssetCollection)obj);
    }

    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }
}