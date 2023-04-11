using System.Text.Json.Nodes;
using NebulaCore.Engine;
using NebulaJsonRpc;
using WatsonWebsocket;

namespace NebulaCore;

public static class NebulaDebugger
{
    private static WatsonWsServer? _server;
    private static Guid? _client;
    private static JsonRpcPeer? _peer;

    public static void StartServer(int port = 3590)
    {
        _server = new WatsonWsServer("127.0.0.1", port);
        _server.MessageReceived += MessageReceived;
        _server.Start();
        
        _peer = new JsonRpcPeer(SendMessage);
        RegisterMethods(_peer);

        Console.WriteLine("Debugger started");
    }

    private static void MessageReceived(object? sender, MessageReceivedEventArgs args)
    {
        if (_peer == null) return;
        _client = args.Client.Guid;
        _peer.ReceiveNetworkMessage(JsonNode.Parse(args.Data)!.AsObject());
    }

    private static void SendMessage(object? sender, JsonObject message)
    {
        if (_server == null || _client == null) return;
        _server.SendAsync((Guid)_client, message.ToJsonString());
    }

    public static void StopServer()
    {
        if (_server != null)
        {
            _server.Stop();
            _server.Dispose();
        }

        if (_peer != null)
        {
            _peer.Close();
        }

        _server = null;
        _client = null;
        _peer = null;
        
        Console.WriteLine("Debugger stopped");
    }
    
    private static void RegisterMethods(IJsonRpcServer server)
    {
        server.RegisterMethod("GetEngineInfo", () => new JsonObject
        {
            { "nebulaVersion", Nebula.NebulaVersion },
            { "glVersion", Nebula.GlVersion },
            { "glRenderer", Nebula.GlRenderer },
        });

        server.RegisterMethod("CreateAndLoadProject", param =>
        {
            var project = Project.Create(param!["path"]!.GetValue<string>(), param["name"]!.GetValue<string>(), param["bundleId"]!.GetValue<string>());
            Nebula.ActiveProject = project;
            return project.Serialize();
        });

        server.RegisterMethod("LoadProject", param =>
        {
            var project = Project.Open(param!["path"]!.GetValue<string>());
            Nebula.ActiveProject = project;
            return project.Serialize();
        });
        
        server.RegisterMethod("UnloadProject", () => Nebula.ActiveProject = null);

        server.RegisterMethod("SaveProject", () => Nebula.ActiveProject?.Save());
    }
}
