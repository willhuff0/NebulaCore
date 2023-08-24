using Nebula.Core;

namespace Nebula.BuiltIn.Assets;

[AssetLoader("material")]
public class Material : Asset, IAssetLoader
{
    public static Asset? LoadAsset(Project project, AssetDefinition definition, bool immediateEnterScope)
    {
        var shader = Guid.Parse(definition.Properties!["shader"]!.GetValue<string>());
        var textures = definition.Properties?["textures"]?.AsObject().ToDictionary(pair => int.Parse(pair.Key), pair => Guid.Parse(pair.Value!.GetValue<string>())) ?? new Dictionary<int, Guid>();

        var parameters = definition.Properties?["parameters"]?.AsObject().ToDictionary(pair => int.Parse(pair.Key), pair => pair.Value!.GetValue<object>()) ?? new Dictionary<int, object>();

        return new Material(project.Database, definition.Type, definition.Key, (shader, textures), parameters);
    }

    private readonly Dictionary<int, object> _parameters;

    private Material(AssetDatabase database, string type, Guid key, (Guid shader, Dictionary<int, Guid> textures) referenceIntermediary, Dictionary<int, object> parameters) : base(database, type, key)
    {
        _referenceIntermediary = referenceIntermediary;
        _parameters = parameters;
    }
    
    private (Guid shader, Dictionary<int, Guid> textures)? _referenceIntermediary;

    public Shader Shader = null!;
    private Dictionary<int, Texture> _textures = null!;

    protected override HashSet<Asset> GatherDependencies() => Shader.Dependencies.Concat(_textures.Values.SelectMany(texture => texture.Dependencies)).ToHashSet();

    public override void LookupReferences()
    {
        Shader = Database.GetLoadedAsset<Shader>(_referenceIntermediary!.Value.shader)!;
        _textures = _referenceIntermediary!.Value.textures.ToDictionary(pair => pair.Key, pair => Database.GetLoadedAsset<Texture>(pair.Value)!);
        
        _referenceIntermediary = null;
    }

    public void Bind()
    {
        Shader.Bind();
        foreach (var (location, value) in _parameters) Shader.SetUniform(location, value);
        foreach (var (binding, texture) in _textures) texture.Bind(binding);
    }
}