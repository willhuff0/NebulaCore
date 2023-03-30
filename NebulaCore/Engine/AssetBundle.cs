using System.Reflection;
using System.Text.Json.Nodes;

namespace NebulaCore.Engine;

public class AssetBundle
{
    private Project _project;
    private Dictionary<string, AssetGroup> _assetGroups;

    public AssetBundle(Project project, JsonObject json)
    {
        _project = project;
        _assetGroups = new Dictionary<string, AssetGroup>();
        ReloadAssetDefinitions();
        ImportAssets(json);
    }

    public JsonObject Serialize()
    {
        var json = new JsonObject();
        var groups = json["groups"] = new JsonObject();
        foreach (var assetGroup in _assetGroups)
        {
            groups[assetGroup.Key] = assetGroup.Value.Serialize();
        }

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
                    if (!_assetGroups.TryGetValue(assetDefinitionAttribute.GroupName, out var assetGroup))
                    {
                         _assetGroups[assetDefinitionAttribute.GroupName] = new AssetGroup(_project, assetDefinitionAttribute.GroupName, constructor);
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
        var jsonGroups = json["groups"];
        if (jsonGroups == null) return;
        foreach (var jsonGroup in jsonGroups.AsObject())
        {
            var jsonGroupAssets = jsonGroup.Value;
            if (jsonGroupAssets == null) continue;

            var assetType = jsonGroup.Key;
            if (!_assetGroups.TryGetValue(assetType, out var assetGroup))
            {
                Console.WriteLine($"Skipping assets from {assetType} because no definition was specified");
                continue;
            }
            
            assetGroup.ImportAssets(jsonGroupAssets.AsObject());
        }
    }

    public TAsset? GetAsset<TAsset> (string group, Guid guid) where TAsset : Asset => _assetGroups[group].GetAsset(guid) as TAsset;
    public TRuntimeAsset? GetRuntimeAsset<TRuntimeAsset> (string group, Guid guid) where TRuntimeAsset : RuntimeAsset => _assetGroups[group].GetRuntimeAsset(guid) as TRuntimeAsset;

    public Guid AddAsset(string group, Asset asset) => _assetGroups[group].AddAsset(asset);

    public void RemoveAsset(string group, Guid guid) => _assetGroups[group].RemoveAsset(guid);
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
        foreach (var asset in _assets)
        {
            json[asset.Key.ToString()] = asset.Value.Serialize();
        }

        return json;
    }

    public void SetAssetDefinition(ConstructorInfo constructor) => _constructor = constructor;
    
    private bool _isLoaded = false;

    public Asset? GetAsset(Guid guid) => _assets[guid];
    public RuntimeAsset? GetRuntimeAsset(Guid guid) => _runtimeAssets[guid];

    public void ImportAssets(JsonObject json)
    {
        int i = 0;
        foreach (var jsonAsset in json)
        {
            var jsonAssetValue = jsonAsset.Value;
            if (jsonAssetValue == null) continue;

            _assets[Guid.Parse(jsonAsset.Key)] = (_constructor.Invoke(new object?[] { _project, jsonAssetValue }) as Asset)!;
            i++;
        }
        Console.WriteLine($"Imported {i} {_groupName}");
    }
    
    public Guid AddAsset(Asset asset)
    {
        var guid = Guid.NewGuid();
        _assets[guid] = asset;
        return guid;
    }

    public void RemoveAsset(Guid guid) => _assets.Remove(guid);

    public async Task Load()
    {
        if (_isLoaded) return;
        
        _runtimeAssets = new Dictionary<Guid, RuntimeAsset>(_assets.Count);

        var tasks = new Task[_assets.Count];
        int i = 0;
        foreach (var entry in _assets)
        {
            tasks[i] = Task.Run(async () =>
            {
                var result = await entry.Value.Load();
                if (result != null) _runtimeAssets[entry.Key] = result;
            });
            i++;
        }
        await Task.WhenAll(tasks);
        
        tasks = new Task[_runtimeAssets.Count];
        i = 0;
        foreach (var entry in _runtimeAssets)
        {
            tasks[i] = Task.Run(async () => await entry.Value.AssignRuntimeReferences());
            i++;
        }
        await Task.WhenAll(tasks);
        
        _isLoaded = true;
    }

    public async Task Unload()
    {
        if (!_isLoaded) return;

        var tasks = new Task[_runtimeAssets.Count];
        int i = 0;
        foreach (var entry in _runtimeAssets)
        {
            tasks[i] = Task.Run(async () => await entry.Value.Unload());
            i++;
        }
        await Task.WhenAll(tasks);

        _isLoaded = false;
    }
}
