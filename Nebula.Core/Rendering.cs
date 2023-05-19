using Nebula.Graphics;

namespace Nebula.Core;

public static class Renderer
{
    public static void Frame(FrameArgs frameArgs)
    {
        GL.Clear(GL.COLOR_BUFFER_BIT | GL.DEPTH_BUFFER_BIT);
        
        frameArgs.Root._onBeginFrame(frameArgs);

        var renderArgs = new RenderArgs(frameArgs, false);
        frameArgs.Root._onRender(renderArgs);
        
        frameArgs.Root._onEndFrame(frameArgs);
    }
}

public class FrameArgs
{
    public readonly Node Root;

    public readonly (int width, int height) FrameSize;

    public readonly double TimeDelta;

    public readonly InputState Input;

    public FrameArgs(Node root, (int width, int height) frameSize, double timeDelta, InputState input)
    {
        Root = root;
        FrameSize = frameSize;
        TimeDelta = timeDelta;
        Input = input;
    }
}

public class RenderArgs
{
    public readonly FrameArgs FrameArgs;
    
    public readonly bool IsForShadows;

    public RenderArgs(FrameArgs frameArgs, bool isForShadows)
    {
        FrameArgs = frameArgs;
        IsForShadows = isForShadows;
    }
}