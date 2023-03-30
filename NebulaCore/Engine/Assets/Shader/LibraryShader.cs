using System.Text.Json.Nodes;

namespace NebulaCore.Engine.Assets;

[AssetDefinition("libraryShaders")]
public class LibraryShader : FileAsset
{
    private static Dictionary<string, LibraryShader> _idLookup;

    private string _id;
    private string _content;
    
    public LibraryShader(Project project, JsonNode json) : base(project, json)
    {
        _id = json["id"].GetValue<string>();
        _content = File.ReadAllText(Path.Join(project.Root, path));
        _idLookup[_id] = this;
    }

    public static LibraryShader? FindById(string id) => _idLookup[id];

    public string Content => _content;

    public override Task<RuntimeAsset?> Load()
    {
        return Task.FromResult<RuntimeAsset?>(null);
    }
}