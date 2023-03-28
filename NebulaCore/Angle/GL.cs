using System.Runtime.InteropServices;

namespace NebulaCore.Angle;

public static unsafe class GL
{
#if WINDOWS
    private const string dll = "LibGLESv2.dll"
#else
    private const string dll = "LibGLESv2.dylib";
#endif

    public const uint VERSION = 0x1f02;
    public const uint RENDERER = 0x1f01;

    public const uint DEPTH_BUFFER_BIT = 0x00000100;
    public const uint STENCIL_BUFFER_BIT = 0x00000400;
    public const uint COLOR_BUFFER_BIT = 0x00004000;

    public const uint TEXTURE_2D = 0x0DE1;
    public const uint CULL_FACE = 0x0B44;
    public const uint BLEND = 0x0BE2;
    public const uint DITHER = 0x0BD0;
    public const uint STENCIL_TEST = 0x0B90;
    public const uint DEPTH_TEST = 0x0B71;
    public const uint SCISSOR_TEST = 0x0C11;
    public const uint POLYGON_OFFSET_FILL = 0x8037;
    public const uint SAMPLE_ALPHA_TO_COVERAGE = 0x809E;
    public const uint SAMPLE_COVERAGE = 0x80A0;

    public const uint FRONT = 0x0404;
    public const uint BACK = 0x0405;
    public const uint FRONT_AND_BACK = 0x0408;

    [DllImport(dll, EntryPoint = "glGetString")]
    public static extern string getString(uint name);
    
    [DllImport(dll, EntryPoint = "glClearColor")]
    public static extern void clearColor(float red, float green, float blue, float alpha);

    [DllImport(dll, EntryPoint = "glClear")]
    public static extern void clear(uint mask);
    
    [DllImport(dll, EntryPoint = "glEnable")]
    public static extern void enable(uint cap);
    
    [DllImport(dll, EntryPoint = "glDisable")]
    public static extern void disable(uint cap);
    
    [DllImport(dll, EntryPoint = "glCullFace")]
    public static extern void cullFace(uint mode);
}