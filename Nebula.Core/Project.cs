using System.Text.Json.Nodes;

namespace Nebula.Core;

public class Project
{
    public const string APP_DIRECTORY = "data";
    public const string USER_DATA_DIRECTORY = "user";
    
    public string Name { get; private set; }
    
    public readonly string RootDirectory;
    public readonly string AppDirectory;
    public readonly string UserDataDirectory;

    public readonly AssetDatabase Database;

    private Project(string path, string name, AssetDatabase database)
    {
        RootDirectory = Path.GetDirectoryName(path)!;
        AppDirectory = Path.Join(RootDirectory, APP_DIRECTORY);
        UserDataDirectory = Path.Join(RootDirectory, USER_DATA_DIRECTORY);
        Name = name;
        Database = database;
    }
    
    public Project(string path, string name)
    {
        RootDirectory = Path.GetDirectoryName(path)!;
        AppDirectory = Path.Join(RootDirectory, APP_DIRECTORY);
        UserDataDirectory = Path.Join(RootDirectory, USER_DATA_DIRECTORY);
        Name = name;

        Database = new AssetDatabase(this);
        
        if (Directory.EnumerateFileSystemEntries(RootDirectory).Any()) throw new Exception($"Failed to create project: Directory must be empty ({RootDirectory})");
        File.WriteAllText(path, new JsonObject() { {"name", name} }.ToJsonString());
    }

    public Project(string path)
    {
        var content = File.ReadAllText(path);
        var json = JsonNode.Parse(content);

        RootDirectory = Path.GetDirectoryName(path)!;
        AppDirectory = Path.Join(RootDirectory, APP_DIRECTORY);
        UserDataDirectory = Path.Join(RootDirectory, USER_DATA_DIRECTORY);
        Name = json!["name"]!.GetValue<string>();
        
        Database = new AssetDatabase(this, json.AsObject());
    }
}