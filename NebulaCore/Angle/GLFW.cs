using System.Runtime.InteropServices;

namespace NebulaCore.Angle;

public static unsafe class Glfw
{
#if WINDOWS
    private const string dll = "glfw3.dll";
#else
    private const string dll = "libglfw.dylib";
#endif

    public delegate void GLFWerrorfun(int error_code, string description);

    public delegate void GLFWframebuffersizefun(void* window, int width, int height);
    
    public const int TRUE = 1;
    public const int FALSE = 0;
    public const int JOYSTICK_HAT_BUTTONS = 0x00050001;
    public const int COCOA_CHDIR_RESOURCES = 0x00051001;
    public const int COCOA_MENUBAR = 0x00051002;

    public const int ANGLE_PLATFORM_TYPE = 0x00050002;
    public const int ANGLE_PLATFORM_TYPE_NONE = 0x00037001;
    public const int ANGLE_PLATFORM_TYPE_OPENGL = 0x00037002;
    public const int ANGLE_PLATFORM_TYPE_OPENGLES = 0x00037003;
    public const int ANGLE_PLATFORM_TYPE_D3D9 = 0x00037004;
    public const int ANGLE_PLATFORM_TYPE_D3D11 = 0x00037005;
    public const int ANGLE_PLATFORM_TYPE_VULKAN = 0x00037007;
    public const int ANGLE_PLATFORM_TYPE_METAL = 0x00037008;

    public const int CONTEXT_CREATION_API = 0x0002200B;
    public const int EGL_CONTEXT_API = 0x00036002;

    public const int CLIENT_API = 0x00022001;
    public const int OPENGL_ES_API = 0x00030002;

    public const int CONTEXT_VERSION_MAJOR = 0x00022002;
    public const int CONTEXT_VERSION_MINOR = 0x00022003;

    public const int SAMPLES = 0x0002100D;

    [DllImport(dll, EntryPoint = "glfwInit")]
    public static extern int init();

    [DllImport(dll, EntryPoint = "glfwTerminate")]
    public static extern void terminate();
    
    [DllImport(dll, EntryPoint = "glfwInitHint")]
    public static extern void initHint(int hint, int value);

    [DllImport(dll, EntryPoint = "glfwGetVersion")]
    public static extern void getVersion(int* major, int* minor, int* rev);

    [DllImport(dll, EntryPoint = "glfwGetVersionString")]
    public static extern string getVersionString();

    [DllImport(dll, EntryPoint = "glfwSetErrorCallback")]
    public static extern GLFWerrorfun setErrorCallback(GLFWerrorfun callback);

    [DllImport(dll, EntryPoint = "glfwMakeContextCurrent")]
    public static extern void makeContextCurrent(void* window);

    [DllImport(dll, EntryPoint = "glfwSetFramebufferSizeCallback")]
    public static extern GLFWframebuffersizefun setFramebufferSizeCallback(void* window, GLFWframebuffersizefun callback);

    [DllImport(dll, EntryPoint = "glfwWindowHint")]
    public static extern void windowHint(int hint, int value);
    
    [DllImport(dll, EntryPoint = "glfwCreateWindow")]
    public static extern void* createWindow(int width, int height, string title, void* monitor, void* share);
        
    [DllImport(dll, EntryPoint = "glfwWindowShouldClose")]
    public static extern int windowShouldClose(void* window);

    [DllImport(dll, EntryPoint = "glfwPollEvents")]
    public static extern void pollEvents();

    [DllImport(dll, EntryPoint = "glfwSwapBuffers")]
    public static extern void swapBuffers(void* window);

    [DllImport(dll, EntryPoint = "glfwDestroyWindow")]
    public static extern void destroyWindow(void* window);
}