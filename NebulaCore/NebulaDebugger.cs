using System.Net;
using System.Net.WebSockets;
using System.Text.Json.Nodes;
using NebulaCore.Engine;
using WatsonWebsocket;

namespace NebulaCore;

public static class NebulaDebugger
{
    private static WatsonWsServer? _server;

    public static void StartServer(int port = 3590)
    {
        _server = new WatsonWsServer("127.0.0.1", port);
        _server.MessageReceived += MessageReceived;
        _server.Start();

        Console.WriteLine("Debugger started");
    }

    private static void MessageReceived(object? sender, MessageReceivedEventArgs args)
    {
        
    }

    public static void StopServer()
    {
        if (_server != null)
        {
            _server.Stop();
            _server.Dispose();
            _server = null;
        }
        
        Console.WriteLine("Debugger stopped");
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
