using System.Reflection;
using System.Text.Json.Nodes;

namespace Nebula.Engine.Assets;

[AssetDefinition("behaviors")]
public class BehaviorAsset : Asset
{
    private string _class;
    public string Class => _class;
    
    public BehaviorAsset(Project project, JsonNode json) : base(project, json)
    {
        _class = json["class"]!.GetValue<string>();
    }

    public override JsonObject Serialize() => new JsonObject()
    {
        { "class", _class }
    };

    public override Task<RuntimeAsset?> Load()
    {
        return Task.FromResult<RuntimeAsset?>(null);
    }

    public Behavior? CreateInstance(JsonNode? json)
    {
        var type = Assembly.GetCallingAssembly().GetType(_class);
        if (type == null) return null;
        var instance = Activator.CreateInstance(type) as Behavior;
        if (instance == null) return null;

        if (json != null)
        {
            
            foreach (var (key, value) in json.AsObject())
            {
                v
                behaviorType.GetField(key)?.SetValue(behaviorInstance, value);
            }
        }
    }
}

public abstract class BehaviorVariable
{
    public Project Project;
    public string Name;

    public BehaviorVariable(Project project, string name, JsonObject json)
    {
        Name = name;
        Project = project;
    }
    
    public static BehaviorVariable ParseBehaviorVariable(Project project, JsonObject json)
    {
        
    }

    public abstract object? Get(RuntimeScene scene);
}

public class AssetBehaviorVariable : BehaviorVariable
{
    private string _group;
    private Guid _guid;

    public AssetBehaviorVariable(Project project, string name, JsonObject json) : base(project, name, json)
    {
        _group = json["group"]!.GetValue<string>();
        _guid = Guid.Parse(json["guid"]!.GetValue<string>());
    }

    public override object? Get(RuntimeScene scene) => Project.Bundle.GetRuntimeAsset<RuntimeAsset>(_group, _guid);
}

public class NodeBehaviorVariable : BehaviorVariable
{
    private Guid _guid;

    public AssetBehaviorVariable(Project project, string name, JsonObject json) : base(project, name, json)
    {
        _guid = Guid.Parse(json["guid"]!.GetValue<string>());
    }

    public override object? Get(RuntimeScene scene)
    {
        scene.
    }
}