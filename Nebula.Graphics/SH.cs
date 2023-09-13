// ReSharper disable InconsistentNaming
namespace Nebula.Graphics;

public static partial class SH
{
#if WINDOWS
    private const string dll = ".dll";
#else
    private const string dll = ".dylib";
#endif
    
    public enum ShShaderSpec
    {
        SH_GLES2_SPEC,
        SH_WEBGL_SPEC,

        SH_GLES3_SPEC,
        SH_WEBGL2_SPEC,

        SH_GLES3_1_SPEC,
        SH_WEBGL3_SPEC,

        SH_GLES3_2_SPEC,

        SH_GL_CORE_SPEC,
        SH_GL_COMPATIBILITY_SPEC,
    }

    public enum ShShaderOutput
    {
        // ESSL output only supported in some configurations.
        SH_ESSL_OUTPUT = 0x8B45,

        // GLSL output only supported in some configurations.
        SH_GLSL_COMPATIBILITY_OUTPUT = 0x8B46,
        // Note: GL introduced core profiles in 1.5.
        SH_GLSL_130_OUTPUT      = 0x8B47,
        SH_GLSL_140_OUTPUT      = 0x8B80,
        SH_GLSL_150_CORE_OUTPUT = 0x8B81,
        SH_GLSL_330_CORE_OUTPUT = 0x8B82,
        SH_GLSL_400_CORE_OUTPUT = 0x8B83,
        SH_GLSL_410_CORE_OUTPUT = 0x8B84,
        SH_GLSL_420_CORE_OUTPUT = 0x8B85,
        SH_GLSL_430_CORE_OUTPUT = 0x8B86,
        SH_GLSL_440_CORE_OUTPUT = 0x8B87,
        SH_GLSL_450_CORE_OUTPUT = 0x8B88,

        // Prefer using these to specify HLSL output type:
        SH_HLSL_3_0_OUTPUT       = 0x8B48,  // D3D 9
        SH_HLSL_4_1_OUTPUT       = 0x8B49,  // D3D 11
        SH_HLSL_4_0_FL9_3_OUTPUT = 0x8B4A,  // D3D 11 feature level 9_3

        // Output SPIR-V for the Vulkan backend.
        SH_SPIRV_VULKAN_OUTPUT = 0x8B4B,

        // Output for MSL
        SH_MSL_METAL_OUTPUT = 0x8B4D,
    }
    
    //public static partial void Initialize()
}