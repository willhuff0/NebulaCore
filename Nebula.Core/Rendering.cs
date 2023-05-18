namespace Nebula.Core;

public class Rendering
{
    
}

public class FrameArgs
{
    public int FrameWidth { get; private set; }
    public int FrameHeight { get; private set; }
    
    public float TimeDelta { get; private set; }
    
    public InputState Input { get; private set; }

    public FrameArgs(int frameWidth, int frameHeight, float timeDelta, InputState input)
    {
        FrameWidth = frameWidth;
        FrameHeight = frameHeight;
        TimeDelta = timeDelta;
        Input = input;
    }
}