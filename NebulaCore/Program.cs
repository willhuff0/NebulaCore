using NebulaCore.Angle;

namespace NebulaCore;

public unsafe class Program
{
    private static void glfwErrorCallback(int error_code, string description)
    {
        Console.WriteLine($"GLFW error {error_code}: {description}");
    }

    private static void glfwFramebufferSizeCallback(void* window, int width, int height)
    {
        GL.Viewport(0, 0, width, height);
    }
    
    public static int Main(string[] args)
    {
        Glfw.InitHint(Glfw.ANGLE_PLATFORM_TYPE, Glfw.ANGLE_PLATFORM_TYPE_METAL);
        if (Glfw.Init() == Glfw.FALSE)
        {
            Console.WriteLine("Failed to initialize GLFW");
            return 1;
        }

        Glfw.SetErrorCallback(glfwErrorCallback);
        
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
            return 2;
        }
        
        Glfw.MakeContextCurrent(window);

        Glfw.SetFramebufferSizeCallback(window, glfwFramebufferSizeCallback);
        
        Console.WriteLine($"Nebula version: {"0.1.0"}");
        //Console.WriteLine($"GLFW version: {Glfw.getVersionString()}");
        Console.WriteLine($"GL version: {GL.GetString(GL.VERSION)}");
        Console.WriteLine($"GL renderer: {GL.GetString(GL.RENDERER)}");
        
        GL.Viewport(0, 0, 1024, 768);
        GL.Enable(GL.DEPTH_TEST);
        GL.Enable(GL.CULL_FACE);
        GL.CullFace(GL.BACK);
        
        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        GL.Clear(GL.COLOR_BUFFER_BIT | GL.DEPTH_BUFFER_BIT);

        while (Glfw.WindowShouldClose(window) == Glfw.FALSE)
        {
            Glfw.PollEvents();
            Glfw.SwapBuffers(window);
        }
        
        Glfw.DestroyWindow(window);
        Glfw.Terminate();
        return 0;
    }
}