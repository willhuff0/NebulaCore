using System.Text.Json.Nodes;

namespace NebulaCore.Engine;

public class Project
{
    public string Root { get; private set; }
    public AssetBundle Bundle { get; private set; }
    
    public string Name { get; private set; }
    public string BundleId { get; private set; }

    private Project(string root, JsonObject json)
    {
        Root = root;
        Bundle = new AssetBundle(this, json["bundle"]!.AsObject());
        Name = json["name"]!.GetValue<String>();
        BundleId = json["bundleId"]!.GetValue<String>();
    }

    private Project(string root, string name, string bundleId)
    {
        Root = root;
        Name = name;
        BundleId = bundleId;
        Bundle = new AssetBundle(this);
    }

    public JsonObject Serialize() => new JsonObject()
    {
        {"name", Name},
        {"bundleId", BundleId},
        {"bundle", Bundle.Serialize()},
    };

    public void Save()
    {
        File.WriteAllText(Root, Serialize().ToJsonString());
    }

    public static Project Open(string path)
    {
        var content = File.ReadAllText(path);
        var json = JsonNode.Parse(content)!;
        return new Project(path, json.AsObject());
    }

    public static Project Create(string path, string name, string bundleId)
    {
        var project = new Project(path, name, bundleId);
        project.Save();
        return project;
    }
}