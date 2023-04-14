using System.Reflection;
using System.Text.Json.Nodes;

namespace NebulaCore.Engine;

public class AssetBundle
{
    private Project _project;
    private Dictionary<string, AssetGroup> _groups;
    //private Dictionary<Guid, int> _collections;

    public AssetBundle(Project project, JsonObject json)
    {
        _project = project;
        _groups = new Dictionary<string, AssetGroup>();
        //_collections = json["collections"]?.GetValue<Dictionary<string, int>?>()?.ToDictionary(pair => Guid.Parse(pair.Key), pair => pair.Value) ?? new Dictionary<Guid, int>();
        ReloadAssetDefinitions();
        ImportAssets(json);
    }

    public AssetBundle(Project project)
    {
        _project = project;
        _groups = new Dictionary<string, AssetGroup>();
        //_collections = new Dictionary<Guid, int>();
        ReloadAssetDefinitions();
    }

    public JsonObject Serialize()
    {
        var json = new JsonObject();

        //_collections = _collections.ToDictionary(pair => pair.Key, _ => 0);

        var groups = new JsonObject();
        foreach (var (groupKey, group) in _groups)
        {
            groups.Add(groupKey, group.Serialize());
        }
        json.Add("groups", groups);

        // var collections = new JsonObject();
        // foreach (var (collectionKey, collectionCount) in _collections)
        // {
        //     collections.Add(collectionKey.ToString(), collectionCount);
        // }
        // json.Add("collections", collections);

        return json;
    }
    
    public void ReloadAssetDefinitions()
    {
        foreach (var type in Assembly.GetCallingAssembly().GetTypes())
        {
            var assetDefinitionAttribute = type.GetCustomAttribute<AssetDefinitionAttribute>(false);
            if (assetDefinitionAttribute != null)
            {
                var constructor = type.GetConstructor(new[] { typeof(Project), typeof(JsonNode) });
                if (constructor != null)
                {
                    if (!_groups.TryGetValue(assetDefinitionAttribute.GroupName, out var assetGroup))
                    {
                         _groups[assetDefinitionAttribute.GroupName] = new AssetGroup(_project, assetDefinitionAttribute.GroupName, constructor);
                    }
                    else
                    {
                        assetGroup.SetAssetDefinition(constructor);
                    }
                }
            }
        }
    }
    
    public void ImportAssets(JsonObject json)
    {
        int i = 0;
        var importResult = new Dictionary<string, int>();
        
        var jsonGroups = json["groups"];
        if (jsonGroups == null) return;
        foreach (var jsonGroup in jsonGroups.AsObject())
        {
            var jsonGroupAssets = jsonGroup.Value;
            if (jsonGroupAssets == null) continue;

            var assetType = jsonGroup.Key;
            if (!_groups.TryGetValue(assetType, out var assetGroup))
            {
                Console.WriteLine($"Skipping assets from {assetType} because no definition was specified");
                continue;
            }
            
            importResult.Add(jsonGroup.Key, i += assetGroup.ImportAssets(jsonGroupAssets.AsObject()));
        }
        
        Console.WriteLine($"Imported {i} total assets: " + String.Join(", ", importResult.Select(pair => $"{pair.Value} {pair.Key}")));
    }

    public TAsset? GetAsset<TAsset> (string group, Guid guid) where TAsset : Asset => _groups[group].GetAsset(guid) as TAsset;
    public TRuntimeAsset? GetRuntimeAsset<TRuntimeAsset> (string group, Guid guid) where TRuntimeAsset : RuntimeAsset => _groups[group].GetRuntimeAsset(guid) as TRuntimeAsset;

    public Guid AddAsset(string group, Asset asset) => _groups[group].AddAsset(asset);

    public void RemoveAsset(string group, Guid guid) => _groups[group].RemoveAsset(guid);

    public async Task Load(Guid? collection = null)
    {
        await Task.WhenAll(_groups.Select(async pair => await pair.Value.Load(collection)));
    }

    public async Task Unload(Guid? collection = null)
    {
        await Task.WhenAll(_groups.Select(async pair => await pair.Value.Unload(collection)));
    }
}

public class AssetGroup
{
    private Project _project;
    
    private string _groupName;
    private ConstructorInfo _constructor;
    
    private Dictionary<Guid, Asset> _assets;
    private Dictionary<Guid, RuntimeAsset> _runtimeAssets;

    public AssetGroup(Project project, string groupName, ConstructorInfo constructor)
    {
        _project = project;
        _groupName = groupName;
        _constructor = constructor;
        _assets = new Dictionary<Guid, Asset>();
        _runtimeAssets = new Dictionary<Guid, RuntimeAsset>();
    }

    public JsonObject Serialize()
    {
        var json = new JsonObject();
        foreach (var (assetKey, asset) in _assets)
        {
            json.Add(assetKey.ToString(), asset.Serialize());
        }
        return json;
    }

    public void SetAssetDefinition(ConstructorInfo constructor) => _constructor = constructor;

    public Asset? GetAsset(Guid guid)
    {
        if (_assets.TryGetValue(guid, out var asset))
        {
            return asset;
        }

        Console.WriteLine($"Tried to get an asset that does not exist: {guid}");
        return null;
    }

    public async Task<RuntimeAsset?> GetRuntimeAsset(Guid guid)
    {
        if (_runtimeAssets.TryGetValue(guid, out var runtimeAsset))
        {
            return runtimeAsset;
        }

        if (_assets.TryGetValue(guid, out var asset))
        {
            Console.WriteLine($"Tried to get a runtime asset that is not loaded: {guid}");
            return await asset.Load();
        }

        Console.WriteLine($"Tried to get a runtime asset that does not exist: {guid}");
        return null;
    }

    public int ImportAssets(JsonObject json)
    {
        int i = 0;
        foreach (var jsonAsset in json)
        {
            var jsonAssetValue = jsonAsset.Value;
            if (jsonAssetValue == null) continue;

            _assets[Guid.Parse(jsonAsset.Key)] = (_constructor.Invoke(new object?[] { _project, jsonAssetValue }) as Asset)!;
            i++;
        }

        return i;
    }
    
    public Guid AddAsset(Asset asset)
    {
        var guid = Guid.NewGuid();
        _assets[guid] = asset;
        return guid;
    }

    public void RemoveAsset(Guid guid) => _assets.Remove(guid);

    public async Task<Dictionary<Guid, RuntimeAsset>> Load(Guid? collection)
    {
        var runtimeAssets = new Dictionary<Guid, RuntimeAsset>();
        
        await Task.WhenAll(_assets.Select(pair => Task.Run(async () =>
        {
            if (collection != null && !pair.Value.IsAssociated((Guid)collection)) return;
            var result = await pair.Value.Load();
            if (result != null) runtimeAssets.Add(pair.Key, result);
        })));
        
        await Task.WhenAll(runtimeAssets.Select(pair => Task.Run(async () => await pair.Value.AssignRuntimeReferences())));

        _runtimeAssets = (Dictionary<Guid, RuntimeAsset>)_runtimeAssets.Concat(runtimeAssets);
        return runtimeAssets;
    }

    public async Task Unload(Guid? collection)
    {
        await Task.WhenAll(_runtimeAssets.Select(pair => Task.Run(async () =>
        {
            if (collection != null && !pair.Value.IsAssociated((Guid)collection)) return;
            await pair.Value.Unload();
        })));
    }
}
