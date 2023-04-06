using System.Runtime.InteropServices;

namespace NebulaCore.Angle;

public static unsafe partial class Glfw
{
#if WINDOWS
    private const string dll = "glfw3.dll";
#else
    private const string dll = "libglfw.dylib";
#endif

    public delegate void GLFWerrorfun(int error_code, string description);

    public delegate void GLFWframebuffersizefun(void* window, int width, int height);

    public delegate void GLFWkeyfun(void* window, int key, int scancode, int action, int mods);

    public delegate void GLFWmousebuttonfun(void* window, int button, int action, int mods);

    public delegate void GLFWscrollfun(void* window, double xoffset, double yoffset);

    public delegate void GLFWcursorposfun(void* window, double xpos, double ypos);

    public delegate void voidfocusfun(void* window, int focused);
    
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

    public const int RELEASE = 0;
    public const int PRESS = 1;
    public const int REPEAT = 2;

    public const int CURSOR = 0x00033001;
    public const int STICKY_KEYS = 0x00033002;
    public const int STICKY_MOUSE_BUTTONS = 0x00033003;
    public const int LOCK_KEY_MODS = 0x00033004;
    public const int RAW_MOUSE_MOTION = 0x00033005;
    public const int CURSOR_NORMAL = 0x00034001;
    public const int CURSOR_HIDDEN = 0x00034002;
    public const int CURSOR_DISABLED = 0x00034003;

    [LibraryImport(dll, EntryPoint = "glfwInit", StringMarshalling = StringMarshalling.Utf8)]
    [return:MarshalAs(UnmanagedType.I4)]
    public static partial bool Init();

    [LibraryImport(dll, EntryPoint = "glfwTerminate", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void Terminate();
    
    [LibraryImport(dll, EntryPoint = "glfwInitHint", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void InitHint(int hint, int value);

    [LibraryImport(dll, EntryPoint = "glfwGetVersion", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void GetVersion(int* major, int* minor, int* rev);

    [LibraryImport(dll, EntryPoint = "glfwGetVersionString", StringMarshalling = StringMarshalling.Utf8)]
    public static partial string GetVersionString();

    [LibraryImport(dll, EntryPoint = "glfwSetErrorCallback", StringMarshalling = StringMarshalling.Utf8)]
    public static partial GLFWerrorfun SetErrorCallback(GLFWerrorfun callback);

    [LibraryImport(dll, EntryPoint = "glfwMakeContextCurrent", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void MakeContextCurrent(void* window);

    [LibraryImport(dll, EntryPoint = "glfwSetFramebufferSizeCallback", StringMarshalling = StringMarshalling.Utf8)]
    public static partial GLFWframebuffersizefun SetFramebufferSizeCallback(void* window, GLFWframebuffersizefun callback);

    [LibraryImport(dll, EntryPoint = "glfwWindowHint", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void WindowHint(int hint, int value);
    
    [LibraryImport(dll, EntryPoint = "glfwCreateWindow", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void* CreateWindow(int width, int height, string title, void* monitor, void* share);
        
    [LibraryImport(dll, EntryPoint = "glfwWindowShouldClose", StringMarshalling = StringMarshalling.Utf8)]
    [return:MarshalAs(UnmanagedType.I4)]
    public static partial bool WindowShouldClose(void* window);

    [LibraryImport(dll, EntryPoint = "glfwPollEvents", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void PollEvents();

    [LibraryImport(dll, EntryPoint = "glfwSwapBuffers", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void SwapBuffers(void* window);

    [LibraryImport(dll, EntryPoint = "glfwDestroyWindow", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void DestroyWindow(void* window);

    [LibraryImport(dll, EntryPoint = "glfwSetKeyCallback", StringMarshalling = StringMarshalling.Utf8)]
    public static partial GLFWkeyfun SetKeyCallback(void* window, GLFWkeyfun callback);
    
    [LibraryImport(dll, EntryPoint = "glfwSetMouseButtonCallback", StringMarshalling = StringMarshalling.Utf8)]
    public static partial GLFWmousebuttonfun SetMouseButtonCallback(void* window, GLFWmousebuttonfun callback);
    
    [LibraryImport(dll, EntryPoint = "glfwSetScrollCallback", StringMarshalling = StringMarshalling.Utf8)]
    public static partial GLFWscrollfun SetScrollCallback(void* window, GLFWscrollfun callback);

    [LibraryImport(dll, EntryPoint = "glfwSetCursorPosCallback", StringMarshalling = StringMarshalling.Utf8)]
    public static partial GLFWcursorposfun SetCursorPosCallback(void* window, GLFWcursorposfun callback);
    
    [LibraryImport(dll, EntryPoint = "glfwRawMouseMotionSupported", StringMarshalling = StringMarshalling.Utf8)]
    public static partial int RawMouseMotionSupported();
    
    [LibraryImport(dll, EntryPoint = "glfwSetInputMode", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void SetInputMode(void* window, int mode, int value);
    
    [LibraryImport(dll, EntryPoint = "glfwSetWindowFocusCallback", StringMarshalling = StringMarshalling.Utf8)]
    public static partial voidfocusfun SetWindowFocusCallback(void* window, voidfocusfun callback);
}

    /// <summary>
    /// Specifies key codes and modifiers in US keyboard layout.
    /// </summary>
    public enum Key
    {
        /// <summary>
        /// An unknown key.
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// The spacebar.
        /// </summary>
        Space = 32,

        /// <summary>
        /// The apostrophe key.
        /// </summary>
        Apostrophe = 39 /* ' */,

        /// <summary>
        /// The comma key.
        /// </summary>
        Comma = 44 /* , */,

        /// <summary>
        /// The minus key.
        /// </summary>
        Minus = 45 /* - */,

        /// <summary>
        /// The period key.
        /// </summary>
        Period = 46 /* . */,

        /// <summary>
        /// The slash key.
        /// </summary>
        Slash = 47 /* / */,

        /// <summary>
        /// The 0 key.
        /// </summary>
        Number0 = 48,

        /// <summary>
        /// The 0 key; alias for <see cref="Number0"/>
        /// </summary>
        D0 = Number0,

        /// <summary>
        /// The 1 key.
        /// </summary>
        Number1 = 49,

        /// <summary>
        /// The 2 key.
        /// </summary>
        Number2 = 50,

        /// <summary>
        /// The 3 key.
        /// </summary>
        Number3 = 51,

        /// <summary>
        /// The 4 key.
        /// </summary>
        Number4 = 52,

        /// <summary>
        /// The 5 key.
        /// </summary>
        Number5 = 53,

        /// <summary>
        /// The 6 key.
        /// </summary>
        Number6 = 54,

        /// <summary>
        /// The 7 key.
        /// </summary>
        Number7 = 55,

        /// <summary>
        /// The 8 key.
        /// </summary>
        Number8 = 56,

        /// <summary>
        /// The 9 key.
        /// </summary>
        Number9 = 57,

        /// <summary>
        /// The semicolon key.
        /// </summary>
        Semicolon = 59 /* ; */,

        /// <summary>
        /// The equal key.
        /// </summary>
        Equal = 61 /* = */,

        /// <summary>
        /// The A key.
        /// </summary>
        A = 65,

        /// <summary>
        /// The B key.
        /// </summary>
        B = 66,

        /// <summary>
        /// The C key.
        /// </summary>
        C = 67,

        /// <summary>
        /// The D key.
        /// </summary>
        D = 68,

        /// <summary>
        /// The E key.
        /// </summary>
        E = 69,

        /// <summary>
        /// The F key.
        /// </summary>
        F = 70,

        /// <summary>
        /// The G key.
        /// </summary>
        G = 71,

        /// <summary>
        /// The H key.
        /// </summary>
        H = 72,

        /// <summary>
        /// The I key.
        /// </summary>
        I = 73,

        /// <summary>
        /// The J key.
        /// </summary>
        J = 74,

        /// <summary>
        /// The K key.
        /// </summary>
        K = 75,

        /// <summary>
        /// The L key.
        /// </summary>
        L = 76,

        /// <summary>
        /// The M key.
        /// </summary>
        M = 77,

        /// <summary>
        /// The N key.
        /// </summary>
        N = 78,

        /// <summary>
        /// The O key.
        /// </summary>
        O = 79,

        /// <summary>
        /// The P key.
        /// </summary>
        P = 80,

        /// <summary>
        /// The Q key.
        /// </summary>
        Q = 81,

        /// <summary>
        /// The R key.
        /// </summary>
        R = 82,

        /// <summary>
        /// The S key.
        /// </summary>
        S = 83,

        /// <summary>
        /// The T key.
        /// </summary>
        T = 84,

        /// <summary>
        /// The U key.
        /// </summary>
        U = 85,

        /// <summary>
        /// The V key.
        /// </summary>
        V = 86,

        /// <summary>
        /// The W key.
        /// </summary>
        W = 87,

        /// <summary>
        /// The X key.
        /// </summary>
        X = 88,

        /// <summary>
        /// The Y key.
        /// </summary>
        Y = 89,

        /// <summary>
        /// The Z key.
        /// </summary>
        Z = 90,

        /// <summary>
        /// The left bracket(opening bracket) key.
        /// </summary>
        LeftBracket = 91 /* [ */,

        /// <summary>
        /// The backslash.
        /// </summary>
        BackSlash = 92 /* \ */,

        /// <summary>
        /// The right bracket(closing bracket) key.
        /// </summary>
        RightBracket = 93 /* ] */,

        /// <summary>
        /// The grave accent key.
        /// </summary>
        GraveAccent = 96 /* ` */,

        /// <summary>
        /// Non US keyboard layout key 1.
        /// </summary>
        World1 = 161 /* non-US #1 */,

        /// <summary>
        /// Non US keyboard layout key 2.
        /// </summary>
        World2 = 162 /* non-US #2 */,

        /// <summary>
        /// The escape key.
        /// </summary>
        Escape = 256,

        /// <summary>
        /// The enter key.
        /// </summary>
        Enter = 257,

        /// <summary>
        /// The tab key.
        /// </summary>
        Tab = 258,

        /// <summary>
        /// The backspace key.
        /// </summary>
        Backspace = 259,

        /// <summary>
        /// The insert key.
        /// </summary>
        Insert = 260,

        /// <summary>
        /// The delete key.
        /// </summary>
        Delete = 261,

        /// <summary>
        /// The right arrow key.
        /// </summary>
        Right = 262,

        /// <summary>
        /// The left arrow key.
        /// </summary>
        Left = 263,

        /// <summary>
        /// The down arrow key.
        /// </summary>
        Down = 264,

        /// <summary>
        /// The up arrow key.
        /// </summary>
        Up = 265,

        /// <summary>
        /// The page up key.
        /// </summary>
        PageUp = 266,

        /// <summary>
        /// The page down key.
        /// </summary>
        PageDown = 267,

        /// <summary>
        /// The home key.
        /// </summary>
        Home = 268,

        /// <summary>
        /// The end key.
        /// </summary>
        End = 269,

        /// <summary>
        /// The caps lock key.
        /// </summary>
        CapsLock = 280,

        /// <summary>
        /// The scroll lock key.
        /// </summary>
        ScrollLock = 281,

        /// <summary>
        /// The num lock key.
        /// </summary>
        NumLock = 282,

        /// <summary>
        /// The print screen key.
        /// </summary>
        PrintScreen = 283,

        /// <summary>
        /// The pause key.
        /// </summary>
        Pause = 284,

        /// <summary>
        /// The F1 key.
        /// </summary>
        F1 = 290,

        /// <summary>
        /// The F2 key.
        /// </summary>
        F2 = 291,

        /// <summary>
        /// The F3 key.
        /// </summary>
        F3 = 292,

        /// <summary>
        /// The F4 key.
        /// </summary>
        F4 = 293,

        /// <summary>
        /// The F5 key.
        /// </summary>
        F5 = 294,

        /// <summary>
        /// The F6 key.
        /// </summary>
        F6 = 295,

        /// <summary>
        /// The F7 key.
        /// </summary>
        F7 = 296,

        /// <summary>
        /// The F8 key.
        /// </summary>
        F8 = 297,

        /// <summary>
        /// The F9 key.
        /// </summary>
        F9 = 298,

        /// <summary>
        /// The F10 key.
        /// </summary>
        F10 = 299,

        /// <summary>
        /// The F11 key.
        /// </summary>
        F11 = 300,

        /// <summary>
        /// The F12 key.
        /// </summary>
        F12 = 301,

        /// <summary>
        /// The F13 key.
        /// </summary>
        F13 = 302,

        /// <summary>
        /// The F14 key.
        /// </summary>
        F14 = 303,

        /// <summary>
        /// The F15 key.
        /// </summary>
        F15 = 304,

        /// <summary>
        /// The F16 key.
        /// </summary>
        F16 = 305,

        /// <summary>
        /// The F17 key.
        /// </summary>
        F17 = 306,

        /// <summary>
        /// The F18 key.
        /// </summary>
        F18 = 307,

        /// <summary>
        /// The F19 key.
        /// </summary>
        F19 = 308,

        /// <summary>
        /// The F20 key.
        /// </summary>
        F20 = 309,

        /// <summary>
        /// The F21 key.
        /// </summary>
        F21 = 310,

        /// <summary>
        /// The F22 key.
        /// </summary>
        F22 = 311,

        /// <summary>
        /// The F23 key.
        /// </summary>
        F23 = 312,

        /// <summary>
        /// The F24 key.
        /// </summary>
        F24 = 313,

        /// <summary>
        /// The F25 key.
        /// </summary>
        F25 = 314,

        /// <summary>
        /// The 0 key on the key pad.
        /// </summary>
        Keypad0 = 320,

        /// <summary>
        /// The 1 key on the key pad.
        /// </summary>
        Keypad1 = 321,

        /// <summary>
        /// The 2 key on the key pad.
        /// </summary>
        Keypad2 = 322,

        /// <summary>
        /// The 3 key on the key pad.
        /// </summary>
        Keypad3 = 323,

        /// <summary>
        /// The 4 key on the key pad.
        /// </summary>
        Keypad4 = 324,

        /// <summary>
        /// The 5 key on the key pad.
        /// </summary>
        Keypad5 = 325,

        /// <summary>
        /// The 6 key on the key pad.
        /// </summary>
        Keypad6 = 326,

        /// <summary>
        /// The 7 key on the key pad.
        /// </summary>
        Keypad7 = 327,

        /// <summary>
        /// The 8 key on the key pad.
        /// </summary>
        Keypad8 = 328,

        /// <summary>
        /// The 9 key on the key pad.
        /// </summary>
        Keypad9 = 329,

        /// <summary>
        /// The decimal key on the key pad.
        /// </summary>
        KeypadDecimal = 330,

        /// <summary>
        /// The divide key on the key pad.
        /// </summary>
        KeypadDivide = 331,

        /// <summary>
        /// The multiply key on the key pad.
        /// </summary>
        KeypadMultiply = 332,

        /// <summary>
        /// The subtract key on the key pad.
        /// </summary>
        KeypadSubtract = 333,

        /// <summary>
        /// The add key on the key pad.
        /// </summary>
        KeypadAdd = 334,

        /// <summary>
        /// The enter key on the key pad.
        /// </summary>
        KeypadEnter = 335,

        /// <summary>
        /// The equal key on the key pad.
        /// </summary>
        KeypadEqual = 336,

        /// <summary>
        /// The left shift key.
        /// </summary>
        ShiftLeft = 340,

        /// <summary>
        /// The left control key.
        /// </summary>
        ControlLeft = 341,

        /// <summary>
        /// The left alt key.
        /// </summary>
        AltLeft = 342,

        /// <summary>
        /// The left super key.
        /// </summary>
        SuperLeft = 343,

        /// <summary>
        /// The right shift key.
        /// </summary>
        ShiftRight = 344,

        /// <summary>
        /// The right control key.
        /// </summary>
        ControlRight = 345,

        /// <summary>
        /// The right alt key.
        /// </summary>
        AltRight = 346,

        /// <summary>
        /// The right super key.
        /// </summary>
        SuperRight = 347,

        /// <summary>
        /// The menu key.
        /// </summary>
        Menu = 348,

        /// <summary>
        /// The last valid key in this enum.
        /// </summary>
        LastKey = Menu
    }