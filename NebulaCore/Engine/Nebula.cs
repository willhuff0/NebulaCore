using System.Diagnostics;
using System.Numerics;
using NebulaCore.Angle;
using NebulaCore.Engine.Assets;

namespace NebulaCore.Engine;

public static unsafe class Nebula
{
    public static Project? ActiveProject { get; set; }
    public static RuntimeScene? ActiveScene { get; set; }
    
    public static string NebulaVersion { get; private set; }
    public static string GlVersion { get; private set; }
    public static string GlRenderer { get; private set; }
    
    private static void GlfwErrorCallback(int error_code, string description)
    {
        Console.WriteLine($"GLFW error {error_code}: {description}");
    }

    private static int _currentWidth;
    private static int _currentHeight;
    private static void GlfwFramebufferSizeCallback(void* window, int width, int height)
    {
        GL.Viewport(0, 0, _currentWidth = width, _currentHeight = height);
    }
    
    public static void InitializeEngine()
    {
        Glfw.InitHint(Glfw.ANGLE_PLATFORM_TYPE, Glfw.ANGLE_PLATFORM_TYPE_METAL);
        if (!Glfw.Init())
        {
            Console.WriteLine("Failed to initialize GLFW");
            return;
        }

        Glfw.SetErrorCallback(GlfwErrorCallback);
        
        Glfw.WindowHint(Glfw.CLIENT_API, Glfw.OPENGL_ES_API);
        Glfw.WindowHint(Glfw.CONTEXT_CREATION_API, Glfw.EGL_CONTEXT_API);
        Glfw.WindowHint(Glfw.CONTEXT_VERSION_MAJOR, 3);
        Glfw.WindowHint(Glfw.CONTEXT_VERSION_MINOR, 0);
        Glfw.WindowHint(Glfw.SAMPLES, 4);
        var window = Glfw.CreateWindow(1024, 768, "NebulaCore C#", null, null);
        if (window == null)
        {
            Console.WriteLine("Failed to create GLFW window");
            Glfw.Terminate();
            return;
        }
        
        Glfw.MakeContextCurrent(window);

        Glfw.SetFramebufferSizeCallback(window, GlfwFramebufferSizeCallback);
        
        Console.WriteLine($"Nebula version: {NebulaVersion = "0.1.0"}");
        //Console.WriteLine($"GLFW version: {Glfw.getVersionString()}");
        Console.WriteLine($"GL version: {GlVersion = GL.GetString(GL.VERSION)}");
        Console.WriteLine($"GL renderer: {GlRenderer = GL.GetString(GL.RENDERER)}");
        
        GL.Viewport(0, 0, 1024, 768);
        _currentWidth = 1024;
        _currentHeight = 768;
        GL.Enable(GL.DEPTH_TEST);
        GL.Enable(GL.CULL_FACE);
        GL.CullFace(GL.BACK);
        
        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        GL.Clear(GL.COLOR_BUFFER_BIT | GL.DEPTH_BUFFER_BIT);
        
        Input.Setup(window);
        NebulaDebugger.StartServer();
        
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var test = NebulaDebuggerHandler.LoadProject(@"C:\Users\wehuf\home\dev\NebulaCore\TestProject\Test Project.neb");

        while (!Glfw.WindowShouldClose(window))
        {
            var inputState = Input.GetState();
            if (inputState.IsKeyPressed((int)Key.Escape)) Input.UnlockCursor(window);
            
            if (ActiveScene != null)
            {
                var timeDelta = (float) stopwatch.Elapsed.TotalSeconds;
                stopwatch.Restart();
                var frameArgs = new FrameEventArgs(_currentWidth, _currentHeight, timeDelta, inputState);
                ActiveScene.Frame(frameArgs);
            }

            Glfw.PollEvents();
            Glfw.SwapBuffers(window);
        }
        
        NebulaDebugger.StopServer();
        
        Glfw.DestroyWindow(window);
        Glfw.Terminate();
    }
}

public class FrameEventArgs : EventArgs
{
    public int FrameWidth { get; private set; }
    public int FrameHeight { get; private set; }
    
    public float TimeDelta { get; private set; }
    
    public InputState Input { get; private set; }

    public FrameEventArgs(int frameWidth, int frameHeight, float timeDelta, InputState input)
    {
        FrameWidth = frameWidth;
        FrameHeight = frameHeight;
        TimeDelta = timeDelta;
        Input = input;
    }
}

public static unsafe class Input
{
    private static HashSet<int> _mousePressed = new HashSet<int>();
    private static HashSet<int> _mouseHeld = new HashSet<int>();
    private static HashSet<int> _mouseReleased = new HashSet<int>();

    private static HashSet<int> _pressed = new HashSet<int>();
    private static HashSet<int> _held = new HashSet<int>();
    private static HashSet<int> _released = new HashSet<int>();
    
    private static (double, double) _mouseDelta = (0.0, 0.0);

    private static void GlfwCursorPosCallback(void* window, double dx, double dy)
    {
        _mouseDelta.Item1 += dx;
        _mouseDelta.Item2 += dy;
    }

    private static void GlfwMouseButtonCallback(void* window, int button, int action, int mods)
    {
        switch (action)
        {
            case Glfw.PRESS:
                _mousePressed.Add(button);
                _mouseHeld.Add(button);
                break;
            case Glfw.RELEASE:
                _mouseReleased.Add(button);
                _mouseHeld.Remove(button);
                break;
        }
    }
    
    private static void GlfwKeyCallback(void* window, int key, int scancode, int action, int mods)
    {
        switch (action)
        {
            case Glfw.PRESS:
                _pressed.Add(key);
                _held.Add(key);
                break;
            case Glfw.RELEASE:
                _released.Add(key);
                _held.Remove(key);
                break;
        }
    }

    private static void GlfwWindowFocusCallback(void* window, int focused)
    {
        _ResetState();
        if (focused == Glfw.TRUE) LockCursor(window);
    }

    public static void LockCursor(void* window)
    {
        Glfw.SetInputMode(window, Glfw.CURSOR, Glfw.CURSOR_DISABLED);
        if (Glfw.RawMouseMotionSupported() == Glfw.TRUE)
        {
            Glfw.SetInputMode(window, Glfw.RAW_MOUSE_MOTION, Glfw.TRUE);
        }
    }

    public static void UnlockCursor(void* window)
    {
        Glfw.SetInputMode(window, Glfw.CURSOR, Glfw.CURSOR_NORMAL);
    }
    
    public static void Setup(void* window)
    {
        Glfw.SetCursorPosCallback(window, GlfwCursorPosCallback);
        Glfw.SetMouseButtonCallback(window, GlfwMouseButtonCallback);
        
        Glfw.SetKeyCallback(window, GlfwKeyCallback);

        Glfw.SetWindowFocusCallback(window, GlfwWindowFocusCallback);
    }

    public static InputState GetState()
    {
        var state = new InputState(_pressed, _held, _released, _mousePressed, _mouseHeld, _mouseReleased, _mouseDelta);
        _PrepareNextFrame();
        return state;
    }

    private static void _PrepareNextFrame()
    {
        _pressed = new HashSet<int>();
        _released = new HashSet<int>();
        _mousePressed = new HashSet<int>();
        _mouseReleased = new HashSet<int>();
        _mouseDelta = (0.0, 0.0);
    }
    
    private static void _ResetState()
    {
        _PrepareNextFrame();
        _held = new HashSet<int>();
        _mouseHeld = new HashSet<int>();
    }
}

public class InputState
{
    private HashSet<int> _pressed;
    private HashSet<int> _held;
    private HashSet<int> _released;

    private HashSet<int> _mousePressed;
    private HashSet<int> _mouseHeld;
    private HashSet<int> _mouseReleased;

    private (double, double) _mouseDelta;

    public InputState(HashSet<int> pressed, HashSet<int> held, HashSet<int> released, HashSet<int> mousePressed, HashSet<int> mouseHeld, HashSet<int> mouseReleased, (double, double) mouseDelta)
    {
        _pressed = pressed;
        _held = held;
        _released = released;
        _mousePressed = mousePressed;
        _mouseHeld = mouseHeld;
        _mouseReleased = mouseReleased;
        _mouseDelta = mouseDelta;
    }

    public bool IsKeyPressed(int key) => _pressed.Contains(key);
    public bool IsKeyHeld(int key) => _held.Contains(key);
    public bool IsKeyReleased(int key) => _released.Contains(key);
    
    public bool IsMouseButtonPressed(int button) => _mousePressed.Contains(button);
    public bool IsMouseButtonHeld(int button) => _mouseHeld.Contains(button);
    public bool IsMouseButtonReleased(int button) => _mouseReleased.Contains(button);

    private Vector2 MouseDelta => new Vector2((float)_mouseDelta.Item1,(float)_mouseDelta.Item2);
}