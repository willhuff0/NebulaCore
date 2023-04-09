using System.Net;
using System.Net.WebSockets;
using System.Text.Json.Nodes;
using NebulaCore.Engine;

namespace NebulaCore;

public static class NebulaDebugger
{
    private static HttpListener? _listener;
    private static WebSocket? _client;

    public static async void StartServer(int port = 3590)
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://127.0.0.1:{port}/");
        _listener.Start();

        Console.WriteLine("Debugger started");

        while (true)
        {
            
        }
    }

    public static void StopServer()
    {
        
    }
}

public class NebulaDebuggerHandler
{
    public static Dictionary<string, dynamic> GetEngineInfo()
    {
        return new Dictionary<string, dynamic>()
        {
            { "nebulaVersion", Nebula.NebulaVersion },
            { "glVersion", Nebula.GlVersion },
            { "glRenderer", Nebula.GlRenderer },
        };
    }
    
    public static Dictionary<string, dynamic> CreateAndLoadProject(string path, string name, string bundleId)
    {
        var project = Project.Create(path, name, bundleId);
        Nebula.ActiveProject = project;
        return project.Serialize().GetValue<Dictionary<string, dynamic>>();
    }

    public static JsonObject LoadProject(string path)
    {
        var project = Project.Open(path);
        Nebula.ActiveProject = project;
        return project.Serialize();
    }

    public static bool UnloadProject()
    {
        Nebula.ActiveProject = null;
        return true;
    }

    public static void SaveProject()
    {
        Nebula.ActiveProject?.Save();
    }
}
