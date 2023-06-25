using Nebula.Graphics;

namespace Nebula.Core;

public static class Renderer
{
    public static void Frame(FrameArgs frameArgs)
    {
        frameArgs.Root._onBeginFrame(frameArgs);

        var renderArgs = new RenderArgs(frameArgs, false);
        frameArgs.Root._onRender(renderArgs);
        
        frameArgs.Root._onEndFrame(frameArgs);
    }
}

public class FrameArgs
{
    public readonly Node Root;

    public readonly (int width, int height) Size;
    public double Aspect => (double)Size.width / (double)Size.height;

    public readonly double TimeTotal;
    public readonly double TimeDelta;

    public readonly InputState Input;

    public FrameArgs(Node root, (int width, int height) size, double timeTotal, double timeDelta, InputState input)
    {
        Root = root;
        Size = size;
        TimeTotal = timeTotal;
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