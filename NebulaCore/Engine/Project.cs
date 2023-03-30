using System.Text.Json.Nodes;

namespace NebulaCore.Engine;

public abstract class Project
{
    public string Root { get; private set; }
    public AssetBundle Bundle { get; private set; }

    public Project(string root, JsonObject json)
    {
        Root = root;
        Bundle = new AssetBundle(this, json);
    }

    public JsonObject Serialize() => new JsonObject()
    {
        {"bundle", Bundle.Serialize()},
    };
}