using System.Runtime.InteropServices;

namespace NebulaCore.Angle;

using GLuint = UInt32;
using GLenum = UInt32;
using GLubyte = Byte;
using GLfloat = Single;
using GLsizei = Int32;
using GLchar = Char;
using GLint = Int32;

public static unsafe partial class GL
{
#if WINDOWS
    private const string dll = "LibGLESv2.dll";
#else
    private const string dll = "LibGLESv2.dylib";
#endif
    
    public const GLenum FALSE = 0;
    public const GLenum TRUE = 1;

    public const GLenum VERSION = 0x1f02;
    public const GLenum RENDERER = 0x1f01;

    public const GLenum DEPTH_BUFFER_BIT = 0x00000100;
    public const GLenum STENCIL_BUFFER_BIT = 0x00000400;
    public const GLenum COLOR_BUFFER_BIT = 0x00004000;

    public const GLenum TEXTURE_2D = 0x0DE1;
    public const GLenum CULL_FACE = 0x0B44;
    public const GLenum BLEND = 0x0BE2;
    public const GLenum DITHER = 0x0BD0;
    public const GLenum STENCIL_TEST = 0x0B90;
    public const GLenum DEPTH_TEST = 0x0B71;
    public const GLenum SCISSOR_TEST = 0x0C11;
    public const GLenum POLYGON_OFFSET_FILL = 0x8037;
    public const GLenum SAMPLE_ALPHA_TO_COVERAGE = 0x809E;
    public const GLenum SAMPLE_COVERAGE = 0x80A0;

    public const GLenum FRONT = 0x0404;
    public const GLenum BACK = 0x0405;
    public const GLenum FRONT_AND_BACK = 0x0408;

    public const GLenum COMPILE_STATUS = 0x8B81;
    public const GLenum INFO_LOG_LENGTH = 0x8B84;
    public const GLenum SHADER_SOURCE_LENGTH = 0x8B88;
    public const GLenum SHADER_COMPILER = 0x8DFA;

    public const GLenum FRAGMENT_SHADER = 0x8B30;
    public const GLenum VERTEX_SHADER = 0x8B31;
    public const GLenum MAX_VERTEX_ATTRIBS = 0x8869;
    public const GLenum MAX_VERTEX_UNIFORM_VECTORS = 0x8DFB;
    public const GLenum MAX_VARYING_VECTORS = 0x8DFC;
    public const GLenum MAX_COMBINED_TEXTURE_IMAGE_UNITS = 0x8B4D;
    public const GLenum MAX_VERTEX_TEXTURE_IMAGE_UNITS = 0x8B4C;
    public const GLenum MAX_TEXTURE_IMAGE_UNITS = 0x8872;
    public const GLenum MAX_FRAGMENT_UNIFORM_VECTORS = 0x8DFD;
    public const GLenum SHADER_TYPE = 0x8B4F;
    public const GLenum DELETE_STATUS = 0x8B80;
    public const GLenum LINK_STATUS = 0x8B82;
    public const GLenum VALIDATE_STATUS = 0x8B83;
    public const GLenum ATTACHED_SHADERS = 0x8B85;
    public const GLenum ACTIVE_UNIFORMS = 0x8B86;
    public const GLenum ACTIVE_UNIFORM_MAX_LENGTH = 0x8B87;
    public const GLenum ACTIVE_ATTRIBUTES = 0x8B89;
    public const GLenum ACTIVE_ATTRIBUTE_MAX_LENGTH = 0x8B8A;
    public const GLenum SHADING_LANGUAGE_VERSION = 0x8B8C;
    public const GLenum CURRENT_PROGRAM = 0x8B8D;

    [LibraryImport(dll, EntryPoint = "glGetString")]
    public static partial string GetString(GLenum name);
    
    [LibraryImport(dll, EntryPoint = "glClearColor")]
    public static partial void ClearColor(GLfloat red, GLfloat green, GLfloat blue, GLfloat alpha);

    [LibraryImport(dll, EntryPoint = "glClear")]
    public static partial void Clear(GLenum mask);
    
    [LibraryImport(dll, EntryPoint = "glEnable")]
    public static partial void Enable(GLenum cap);
    
    [LibraryImport(dll, EntryPoint = "glDisable")]
    public static partial void Disable(GLenum cap);
    
    [LibraryImport(dll, EntryPoint = "glCullFace")]
    public static partial void CullFace(GLenum mode);

    [LibraryImport(dll, EntryPoint = "glViewport")]
    public static partial void Viewport(int x, int y, GLsizei width, GLsizei height);

    [LibraryImport(dll, EntryPoint = "glCreateShader")]
    public static partial GLuint CreateShader(GLenum shaderType);

    [LibraryImport(dll, EntryPoint = "glShaderSource")]
    public static partial void ShaderSource(GLuint shader, GLsizei count, string[] source, GLint[]? length);
    
    // [LibraryImport(dll, EntryPoint = "glShaderSource")]
    // public static partial void ShaderSource(GLuint shader, GLsizei count, string source, GLint[]? length = null);

    [LibraryImport(dll, EntryPoint = "glCompileShader")]
    public static partial void CompileShader(GLuint shader);

    [LibraryImport(dll, EntryPoint = "glGetShaderiv")]
    public static partial void GetShader(GLuint shader, GLenum pname, out GLint param);

    [LibraryImport(dll, EntryPoint = "glGetShaderInfoLog")]
    public static partial void GetShaderInfoLog(GLuint shader, GLsizei maxLength, out GLsizei length, out string infoLog);

    [LibraryImport(dll, EntryPoint = "glCreateProgram")]
    public static partial GLuint CreateProgram();

    [LibraryImport(dll, EntryPoint = "glAttachShader")]
    public static partial void AttachShader(GLuint program, GLuint shader);

    [LibraryImport(dll, EntryPoint = "glLinkProgram")]
    public static partial void LinkProgram(GLuint program);

    [LibraryImport(dll, EntryPoint = "glDetachShader")]
    public static partial void DetachShader(GLuint program, GLuint shader);

    [LibraryImport(dll, EntryPoint = "glDeleteShader")]
    public static partial void DeleteShader(GLuint shader);

    [LibraryImport(dll, EntryPoint = "glGetProgramiv")]
    public static partial void GetProgram(GLuint program, GLenum pname, out GLint param);
    
    [LibraryImport(dll, EntryPoint = "glGetProgramInfoLog")]
    public static partial void GetProgramInfoLog(GLuint shader, GLsizei maxLength, out GLsizei length, out string infoLog);

    [LibraryImport(dll, EntryPoint = "glDeleteProgram")]
    public static partial void DeleteProgram(GLuint program);

    [LibraryImport(dll, EntryPoint = "glGetActiveUniform")]
    public static partial void GetActiveUniform(GLuint program, GLuint index, GLsizei bufSize, out GLsizei length, out GLint size, out GLenum type, out string name);

    [LibraryImport(dll, EntryPoint = "glGetUniformLocation")]
    public static partial GLint GetUniformLocation(GLuint program, string name);
    
    [LibraryImport(dll, EntryPoint = "glUseProgram")]
    public static partial void UseProgram(GLuint program);

    [LibraryImport(dll, EntryPoint = "glProgramUniform1f")]
    public static partial void ProgramUniform(GLuint program, GLint location, GLfloat v0);
    [LibraryImport(dll, EntryPoint = "glProgramUniform2f")]
    public static partial void ProgramUniform(GLuint program, GLint location, GLfloat v0, GLfloat v1);
    [LibraryImport(dll, EntryPoint = "glProgramUniform3f")]
    public static partial void ProgramUniform(GLuint program, GLint location, GLfloat v0, GLfloat v1, GLfloat v2);
    [LibraryImport(dll, EntryPoint = "glProgramUniform4f")]
    public static partial void ProgramUniform(GLuint program, GLint location, GLfloat v0, GLfloat v1, GLfloat v2, GLfloat v3);
    
    [LibraryImport(dll, EntryPoint = "glProgramUniform1i")]
    public static partial void ProgramUniform(GLuint program, GLint location, GLint v0);
    [LibraryImport(dll, EntryPoint = "glProgramUniform2i")]
    public static partial void ProgramUniform(GLuint program, GLint location, GLint v0, GLint v1);
    [LibraryImport(dll, EntryPoint = "glProgramUniform3i")]
    public static partial void ProgramUniform(GLuint program, GLint location, GLint v0, GLint v1, GLint v2);
    [LibraryImport(dll, EntryPoint = "glProgramUniform4i")]
    public static partial void ProgramUniform(GLuint program, GLint location, GLint v0, GLint v1, GLint v2, GLint v3);
    
    [LibraryImport(dll, EntryPoint = "glProgramUniform1ui")]
    public static partial void ProgramUniform(GLuint program, GLint location, GLuint v0);
    [LibraryImport(dll, EntryPoint = "glProgramUniform2ui")]
    public static partial void ProgramUniform(GLuint program, GLint location, GLuint v0, GLuint v1);
    [LibraryImport(dll, EntryPoint = "glProgramUniform3ui")]
    public static partial void ProgramUniform(GLuint program, GLint location, GLuint v0, GLuint v1, GLuint v2);
    [LibraryImport(dll, EntryPoint = "glProgramUniform4ui")]
    public static partial void ProgramUniform(GLuint program, GLint location, GLuint v0, GLuint v1, GLuint v2, GLuint v3);
    
    [LibraryImport(dll, EntryPoint = "glProgramUniformMatrix4fv")]
    public static partial void ProgramUniformMatrix4(GLuint program, GLint location, GLsizei count, bool transpose, float[] value);
}