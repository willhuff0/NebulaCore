using System.Collections;

namespace NebulaCore.engine;

public class AssetBundle
{
    private Dictionary<string, AssetCollection<IAsset<IRuntimeAsset>, IRuntimeAsset>> _collections;
}

public class AssetCollection<TAsset, TRuntimeAsset> 
    where TAsset : IAsset<TRuntimeAsset>
    where TRuntimeAsset : IRuntimeAsset
{
    private Dictionary<Guid, TAsset> _assets;
    private Dictionary<Guid, TRuntimeAsset> _runtimeAssets;

    private bool _isLoaded = false;

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
                var result = await entry.Value.load();
                if (result != null) _runtimeAssets[entry.Key] = result;
            });

            i++;
        }
        await Task.WhenAll(tasks);
        
        tasks = new Task[_runtimeAssets.Count];
        i = 0;
        foreach (var entry in _runtimeAssets)
        {
            tasks[i] = Task.Run(async () =>
            { 
                await entry.Value.assignReferences();
            });

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
            tasks[i] = Task.Run(async () =>
            {
                entry.Value.unload();
            });
            i++;
        }

    }
}
