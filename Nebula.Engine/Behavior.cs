using System.Text.Json.Nodes;

namespace Nebula.Engine;

public abstract class Behavior
{
    internal abstract Task OnLoad();
    
    internal abstract void OnFrame(FrameEventArgs args);

    internal abstract Task OnUnload();

    public abstract JsonObject Serialize();
}