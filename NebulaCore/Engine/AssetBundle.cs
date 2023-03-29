using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NebulaCore.Engine;

public class AssetBundle<TAssetDefinitions>
    where TAssetDefinitions : IAssetDefinitions
{
    private Dictionary<string, AssetGroup<IAsset<IRuntimeAsset>, IRuntimeAsset>> _assetGroups;

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
                var definition = TAssetDefinitions.GetDefinition(assetType);
                assetGroup = _assetGroups[assetType] = (Activator.CreateInstance(typeof(AssetGroup<,>).MakeGenericType(new[]
                    {
                        definition.Item1, definition.Item2
                    })) as AssetGroup<IAsset<IRuntimeAsset>, IRuntimeAsset>)!;
            }
            
            assetGroup.ImportAssets(jsonGroupAssets.AsObject());
        }
    }

    public TAsset? GetAsset<TAsset> (string group, Guid guid) where TAsset : Asset => _assetGroups[group].GetAsset(guid) as TAsset;
    public TRuntimeAsset? GetRuntimeAsset<TRuntimeAsset> (string group, Guid guid) where TRuntimeAsset : RuntimeAsset => _assetGroups[group].GetRuntimeAsset(guid) as TRuntimeAsset;

    public Guid AddAsset(string group, Asset asset) => _assetGroups[group].AddAsset(asset);

    public void RemoveAsset(string group, Guid guid) => _assetGroups[group].RemoveAsset(guid);
}

public class AssetGroup<TAsset, TRuntimeAsset> 
    where TAsset : IAsset<TRuntimeAsset>
    where TRuntimeAsset : IRuntimeAsset
{
    private Dictionary<Guid, TAsset> _assets;
    private Dictionary<Guid, TRuntimeAsset> _runtimeAssets;

    private bool _isLoaded = false;

    public TAsset? GetAsset(Guid guid) => _assets[guid];
    public TRuntimeAsset? GetRuntimeAsset(Guid guid) => _runtimeAssets[guid];

    public void ImportAssets(JsonObject json)
    {
        foreach (var jsonAsset in json)
        {
            var jsonAssetValue = jsonAsset.Value;
            if (jsonAssetValue == null) continue;

            _assets[Guid.Parse(jsonAsset.Key)] = jsonAssetValue.GetValue<TAsset>();
        }
    }
    
    public Guid AddAsset(TAsset asset)
    {
        var guid = Guid.NewGuid();
        _assets[guid] = asset;
        return guid;
    }

    public void RemoveAsset(Guid guid) => _assets.Remove(guid);

    public async Task Load()
    {
        if (_isLoaded) return;
        
        _runtimeAssets = new Dictionary<Guid, TRuntimeAsset>(_assets.Count);

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
