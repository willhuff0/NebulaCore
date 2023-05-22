using System.Diagnostics;
using System.Runtime.InteropServices;
using Nebula.Graphics;

namespace Nebula.Core;

public static unsafe class Engine
{
    public static Node? RootNode { get; set; }

    public static EngineInformation EngineInfo { get; private set; } = null!;
    public static WindowInformation WindowInfo { get; private set; } = null!;

    private static void* _window;
    
    #region Callbacks
    
    private static void GlfwErrorCallback(int errorCode, string description)
    {
        Console.WriteLine($"GLFW error {errorCode}: {description}");
    }
    
    private static void GlfwFramebufferSizeCallback(void* window, int width, int height)
    {
        WindowInfo = new WindowInformation((width, height));
        GL.Viewport(0, 0, width, height);
    }
    
    #endregion

    public static void Init()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Glfw.InitHint(Glfw.ANGLE_PLATFORM_TYPE, Glfw.ANGLE_PLATFORM_TYPE_VULKAN);
        } 
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Glfw.InitHint(Glfw.ANGLE_PLATFORM_TYPE, Glfw.ANGLE_PLATFORM_TYPE_METAL);
        }
        
        if (!Glfw.Init())
        {
            Console.WriteLine("Failed to initialize GLFW");
            return;
        }

        Glfw.SetErrorCallback(GlfwErrorCallback);
        
        Glfw.WindowHint(Glfw.CLIENT_API, Glfw.OPENGL_ES_API);
        Glfw.WindowHint(Glfw.CONTEXT_CREATION_API, Glfw.EGL_CONTEXT_API);
        Glfw.WindowHint(Glfw.CONTEXT_VERSION_MAJOR, 3);
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Glfw.WindowHint(Glfw.CONTEXT_VERSION_MINOR, 1);
        } 
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Glfw.WindowHint(Glfw.CONTEXT_VERSION_MINOR, 0);
        }
        Glfw.WindowHint(Glfw.SAMPLES, 4);
        _window = Glfw.CreateWindow(1024, 768, "Nebula.Core C#", null, null);
        if (_window == null)
        {
            Console.WriteLine("Failed to create GLFW window");
            Glfw.Terminate();
            return;
        }
        
        Glfw.MakeContextCurrent(_window);
        
        Glfw.SetFramebufferSizeCallback(_window, GlfwFramebufferSizeCallback);

        EngineInfo = EngineInformation.FromCurrent();
        EngineInfo.Print();

        WindowInfo = new WindowInformation((1024, 768));
        GL.Viewport(0, 0, 1024, 768);
        
        GL.Enable(GL.DEPTH_TEST);
        
        GL.Enable(GL.CULL_FACE);
        GL.CullFace(GL.BACK);
        
        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        GL.Clear(GL.COLOR_BUFFER_BIT | GL.DEPTH_BUFFER_BIT);
        
        Input.Setup(_window);
        // Start debugger
    }

    public static void Loop()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        while (!Glfw.WindowShouldClose(_window))
        {
            var inputState = Input.GetState();
            if (inputState.IsKeyPressed((int)Key.Escape)) Input.UnlockCursor(_window);
            
            var timeDelta = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Restart();
            
            GL.Clear(GL.COLOR_BUFFER_BIT | GL.DEPTH_BUFFER_BIT);

            if (RootNode != null)
            {
                var frameArgs = new FrameArgs(RootNode, WindowInfo.Size, timeDelta, inputState);
                Renderer.Frame(frameArgs);
            }

#if DEBUG
            GL.CheckError();
#endif

            Glfw.PollEvents();
            Glfw.SwapBuffers(_window);
        }
    }

    public static void Destroy()
    {
        Glfw.DestroyWindow(_window);
        Glfw.Terminate();
    }
}

public class EngineInformation
{
    public readonly string NebulaVersion;
    public readonly string GlfwVersion;
    public readonly string GlVersion;
    public readonly string GlRenderer;
    
    public EngineInformation(string nebulaVersion, string glfwVersion, string glVersion, string glRenderer)
    {
        NebulaVersion = nebulaVersion;
        GlfwVersion = glfwVersion;
        GlVersion = glVersion;
        GlRenderer = glRenderer;
    }

    public static EngineInformation FromCurrent()
    {
        Glfw.GetVersion(out int major, out int minor, out int rev);
        return new EngineInformation(
            "0.1.0",
            $"{major}.{minor}.{rev}",
            GL.GetString(GL.VERSION),
            GL.GetString(GL.RENDERER)
            );
    }

    public void Print()
    {
        Console.WriteLine($"Nebula version: {NebulaVersion}");
        Console.WriteLine($"GLFW version: {GlfwVersion}");
        Console.WriteLine($"GL version: {GlVersion}");
        Console.WriteLine($"GL renderer: {GlRenderer}");
    }
}

public class WindowInformation
{
    public readonly (int width, int height) Size;
    
    public WindowInformation((int width, int height) size)
    {
        Size = size;
    }
}