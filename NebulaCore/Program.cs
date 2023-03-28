using NebulaCore.Angle;

namespace NebulaCore;

public class Program
{
    private static void glfwErrorCallback(int error_code, string description)
    {
        Console.WriteLine($"GLFW error {error_code}: {description}");
    }
    
    public static unsafe int Main(string[] args)
    {
        Glfw.initHint(Glfw.ANGLE_PLATFORM_TYPE, Glfw.ANGLE_PLATFORM_TYPE_METAL);
        if (Glfw.init() == Glfw.FALSE)
        {
            Console.WriteLine("Failed to initialize GLFW");
            return 1;
        }

        Glfw.setErrorCallback(glfwErrorCallback);
        
        Glfw.windowHint(Glfw.CLIENT_API, Glfw.OPENGL_ES_API);
        Glfw.windowHint(Glfw.CONTEXT_CREATION_API, Glfw.EGL_CONTEXT_API);
        Glfw.windowHint(Glfw.CONTEXT_VERSION_MAJOR, 3);
        Glfw.windowHint(Glfw.CONTEXT_VERSION_MINOR, 0);
        Glfw.windowHint(Glfw.SAMPLES, 4);
        var window = Glfw.createWindow(1024, 768, "NebulaCore C#", null, null);
        if (window == null)
        {
            Console.WriteLine("Failed to create GLFW window");
            Glfw.terminate();
            return 2;
        }
        
        Glfw.makeContextCurrent(window);
        
        Console.WriteLine($"Nebula version: {"0.1.0"}");
        //Console.WriteLine($"GLFW version: {Glfw.getVersionString()}");
        Console.WriteLine($"GL version: {GL.getString(GL.VERSION)}");
        Console.WriteLine($"GL renderer: {GL.getString(GL.RENDERER)}");
        
        GL.enable(GL.DEPTH_TEST);
        GL.enable(GL.CULL_FACE);
        GL.cullFace(GL.BACK);
        
        GL.clearColor(0.0f, 0.0f, 1.0f, 1.0f);
        GL.clear(GL.COLOR_BUFFER_BIT | GL.DEPTH_BUFFER_BIT);

        while (Glfw.windowShouldClose(window) == Glfw.FALSE)
        {
            Glfw.pollEvents();
            Glfw.swapBuffers(window);
        }
        
        Glfw.destroyWindow(window);
        Glfw.terminate();
        return 0;
    }
}