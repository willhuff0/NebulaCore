using System.Net;
using System.Net.Sockets;

namespace NebulaCore;

public static class Debugger
{
    private static TcpListener _listener;

    public static void StartServer()
    {
        _listener = new TcpListener(IPAddress.None, )
    }
}