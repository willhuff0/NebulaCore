namespace Nebula.Core;

public static class Renderer
{
    public static void Frame(FrameArgs args)
    {
        args.Root.OnBeginFrame();
        
        args.Root.OnRender();
        
        args.Root.OnEndFrame();
    }
}

public class FrameArgs
{
    public Node Root { get; private set; }
    
    public (int width, int height) FrameSize { get; private set; }

    public double TimeDelta { get; private set; }

    public InputState Input { get; private set; }

    public FrameArgs(Node root, (int width, int height) frameSize, double timeDelta, InputState input)
    {
        Root = root;
        FrameSize = frameSize;
        TimeDelta = timeDelta;
        Input = input;
    }
}