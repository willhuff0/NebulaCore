using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NebulaCore.Engine.Assets;

public class LibraryShader : FileAsset
{
    private string _content;
    
    public LibraryShader(Project project, JsonNode json) : base(project, json)
    {
    }

    public override JsonObject Serialize() => new JsonObject()
    {

    };
    
    public override Task<IRuntimeAsset?> Load()
    {
        throw new NotImplementedException();
    }
}