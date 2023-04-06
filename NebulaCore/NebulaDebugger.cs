using System.Net;
using System.Net.WebSockets;
using NebulaCore.Engine;
using StreamJsonRpc;

namespace NebulaCore;

public static class NebulaDebugger
{
    private static HttpListener? _listener;
    private static WebSocket? _client;
    private static JsonRpc? rpc;

    public static async void StartServer(int port = 3590)
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://127.0.0.1:{port}/");
        _listener.Start();
        
        Console.WriteLine("Debugger started");

        while (true)
        {
            var context = await _listener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                HttpListenerWebSocketContext webSocketContext;
                try
                {
                    webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
                }
                catch(Exception e)
                {
                    context.Response.StatusCode = 500;
                    context.Response.Close();
                    Console.WriteLine($"Exception: {e}");
                    return;
                }

                if (_client != null) _client.Dispose();
                _client = webSocketContext.WebSocket;
                rpc = new JsonRpc(new WebSocketMessageHandler(_client), new NebulaDebuggerHandler());
                rpc.StartListening();
                Console.WriteLine($"New debugger connection: {context.Request.RemoteEndPoint}");
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }

    public static void StopServer()
    {
        if (_client != null) _client.Dispose();
        _client = null;
        if (rpc != null) rpc.Dispose();
        rpc = null;
        if (_listener != null) _listener.Stop();
        _listener = null;
        
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

    public static Dictionary<string, dynamic> LoadProject(string path)
    {
        var project = Project.Open(path);
        Nebula.ActiveProject = project;
        return project.Serialize().GetValue<Dictionary<string, dynamic>>();
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