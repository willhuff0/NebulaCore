using System.Text.Json.Nodes;

namespace NebulaCore.Engine.Assets;

[AssetDefinition("materials")]
public class Material : Asset
{
    private Guid _shader;
    public Guid Shader => _shader;
    private Dictionary<string, Guid> _textures;
    public Dictionary<string, Guid> Textures => _textures;
    private Dictionary<string, object> _params;

    public Material(Project project, JsonNode json) : base(project, json)
    {
        _shader = Guid.Parse(json["shader"]!.GetValue<string>());
        _textures = new Dictionary<string, Guid>();
        var jsonTextures = json["textures"]?.AsObject();
        if (jsonTextures != null)
        {
            foreach (var (key, jsonTexture) in jsonTextures)
            {
                _textures[key] = Guid.Parse(jsonTexture!.GetValue<string>());
            }
        }

        _params = json["params"]?.GetValue<Dictionary<string, object>>() ?? new Dictionary<string, object>();
    }
    
    public override JsonObject Serialize()
    {
        var json = new JsonObject();
        json["shader"] = JsonValue.Create(_shader);
        if (_textures.Count > 0) json["textures"] = JsonValue.Create(_textures);
        if (_params.Count > 0) json["params"] = JsonValue.Create(_params);
        return json;
    }

    public override Task<RuntimeAsset?> Load()
    {
        return Task.FromResult<RuntimeAsset?>(new RuntimeMaterial(Project, this, _params));
    }
}

public class RuntimeMaterial : RuntimeAsset
{
    private Material? _asset;
    
    private RuntimeRenderShader _shader;
    private Dictionary<string, RuntimeTexture> _textures;
    private Dictionary<string, object> _params;

    public RuntimeMaterial(Project project, Material asset, Dictionary<string, object> _params) : base(project)
    {
        _asset = asset;
        this._params = _params;
    }

    public override Task AssignRuntimeReferences()
    {
        _shader = Project.Bundle.GetRuntimeAsset<RuntimeRenderShader>("shaders", _asset!.Shader)!;
        _textures = new Dictionary<string, RuntimeTexture>();
        foreach (var (key, guid) in _asset!.Textures)
        {
            _textures[key] = Project.Bundle.GetRuntimeAsset<RuntimeTexture>("textures", guid)!;
        }

        _asset = null;
        return base.AssignRuntimeReferences();
    }

    public override Task Unload()
    {
        return Task.CompletedTask;
    }

    public void Bind()
    {
        _shader.Bind();
        foreach (var (key, value) in _params)
        {
            _shader.SetUniform(key, value);
        }

        int i = 0;
        foreach (var (key, texture) in _textures)
        {
            texture.Bind(i);
            _shader.SetUniform($"nebula_material.texture_{key}", i);
            i++;
        }
    }
}