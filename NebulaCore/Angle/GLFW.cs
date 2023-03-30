using System.Runtime.InteropServices;

namespace NebulaCore.Angle;

using GLFWwindow = Void;

public static unsafe partial class Glfw
{
#if WINDOWS
    private const string dll = "glfw3.dll";
#else
    private const string dll = "libglfw.dylib";
#endif

    public delegate void GLFWerrorfun(int error_code, string description);

    public delegate void GLFWframebuffersizefun(GLFWwindow* window, int width, int height);
    
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

    [LibraryImport(dll, EntryPoint = "glfwInit")]
    public static partial int Init();

    [LibraryImport(dll, EntryPoint = "glfwTerminate")]
    public static partial void Terminate();
    
    [LibraryImport(dll, EntryPoint = "glfwInitHint")]
    public static partial void InitHint(int hint, int value);

    [LibraryImport(dll, EntryPoint = "glfwGetVersion")]
    public static partial void GetVersion(int* major, int* minor, int* rev);

    [LibraryImport(dll, EntryPoint = "glfwGetVersionString")]
    public static partial string GetVersionString();

    [LibraryImport(dll, EntryPoint = "glfwSetErrorCallback")]
    public static partial GLFWerrorfun SetErrorCallback(GLFWerrorfun callback);

    [LibraryImport(dll, EntryPoint = "glfwMakeContextCurrent")]
    public static partial void MakeContextCurrent(GLFWwindow* window);

    [LibraryImport(dll, EntryPoint = "glfwSetFramebufferSizeCallback")]
    public static partial GLFWframebuffersizefun SetFramebufferSizeCallback(void* window, GLFWframebuffersizefun callback);

    [LibraryImport(dll, EntryPoint = "glfwWindowHint")]
    public static partial void WindowHint(int hint, int value);
    
    [LibraryImport(dll, EntryPoint = "glfwCreateWindow")]
    public static partial GLFWwindow* CreateWindow(int width, int height, string title, void* monitor, GLFWwindow* share);
        
    [LibraryImport(dll, EntryPoint = "glfwWindowShouldClose")]
    public static partial int WindowShouldClose(GLFWwindow* window);

    [LibraryImport(dll, EntryPoint = "glfwPollEvents")]
    public static partial void PollEvents();

    [LibraryImport(dll, EntryPoint = "glfwSwapBuffers")]
    public static partial void SwapBuffers(GLFWwindow* window);

    [LibraryImport(dll, EntryPoint = "glfwDestroyWindow")]
    public static partial void DestroyWindow(GLFWwindow* window);
}