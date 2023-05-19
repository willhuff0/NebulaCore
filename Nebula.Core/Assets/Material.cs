namespace Nebula.Core.Assets;

[AssetLoader("material")]
public class Material : Asset, IAssetLoader
{
    public static Task<Asset?> LoadAssetAsync(Project project, AssetDefinition definition)
    {
        var shader = Guid.Parse(definition.Properties!["shader"]!.GetValue<string>());
        var textures = definition.Properties?["textures"]?.AsObject().ToDictionary(pair => int.Parse(pair.Key), pair => Guid.Parse(pair.Value!.GetValue<string>())) ?? new Dictionary<int, Guid>();

        var parameters = definition.Properties?["parameters"]?.AsObject().ToDictionary(pair => int.Parse(pair.Key), pair => pair.Value!.GetValue<object>()) ?? new Dictionary<int, object>();

        return Task.FromResult<Asset?>(new Material(definition.Type, definition.Key, (shader, textures), parameters));
    }

    private readonly Dictionary<int, object> _parameters;

    private Material(string type, Guid key, (Guid shader, Dictionary<int, Guid> textures) referenceIntermediary, Dictionary<int, object> parameters) : base(type, key)
    {
        _referenceIntermediary = referenceIntermediary;
        _parameters = parameters;
    }
    
    private (Guid shader, Dictionary<int, Guid> textures)? _referenceIntermediary;

    private Shader _shader = null!;
    private Dictionary<int, Texture> _textures = null!;

    public override void LookupReferences(AssetDatabase database)
    {
        _shader = database.GetAsset<Shader>(_referenceIntermediary!.Value.shader)!;
        _textures = _referenceIntermediary!.Value.textures.ToDictionary(pair => pair.Key, pair => database.GetAsset<Texture>(pair.Value)!);
        
        _referenceIntermediary = null;
    }

    public override void EnterScope()
    {
        
    }

    public override void ExitScope()
    {
        
    }

    public void Bind()
    {
        _shader.Bind();
        foreach (var (location, value) in _parameters) _shader.SetUniform(location, value);
        foreach (var (binding, texture) in _textures) texture.Bind(binding);
    }
}