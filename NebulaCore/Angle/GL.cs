using System.Runtime.InteropServices;

namespace NebulaCore.Angle;

using GLuint = UInt32;
using GLenum = UInt32;
using GLubyte = Byte;
using GLfloat = Single;
using GLsizei = Int32;
using GLchar = Char;
using GLint = Int32;

public static partial class GL
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

    public const GLenum TEXTURE0 = 0x84C0;
    public const GLenum TEXTURE1 = 0x84C1;
    public const GLenum TEXTURE2 = 0x84C2;
    public const GLenum TEXTURE3 = 0x84C3;
    public const GLenum TEXTURE4 = 0x84C4;
    public const GLenum TEXTURE5 = 0x84C5;
    public const GLenum TEXTURE6 = 0x84C6;
    public const GLenum TEXTURE7 = 0x84C7;
    public const GLenum TEXTURE8 = 0x84C8;
    public const GLenum TEXTURE9 = 0x84C9;
    public const GLenum TEXTURE10 = 0x84CA;
    public const GLenum TEXTURE11 = 0x84CB;
    public const GLenum TEXTURE12 = 0x84CC;
    public const GLenum TEXTURE13 = 0x84CD;
    public const GLenum TEXTURE14 = 0x84CE;
    public const GLenum TEXTURE15 = 0x84CF;
    public const GLenum TEXTURE16 = 0x84D0;
    public const GLenum TEXTURE17 = 0x84D1;
    public const GLenum TEXTURE18 = 0x84D2;
    public const GLenum TEXTURE19 = 0x84D3;
    public const GLenum TEXTURE20 = 0x84D4;
    public const GLenum TEXTURE21 = 0x84D5;
    public const GLenum TEXTURE22 = 0x84D6;
    public const GLenum TEXTURE23 = 0x84D7;
    public const GLenum TEXTURE24 = 0x84D8;
    public const GLenum TEXTURE25 = 0x84D9;
    public const GLenum TEXTURE26 = 0x84DA;
    public const GLenum TEXTURE27 = 0x84DB;
    public const GLenum TEXTURE28 = 0x84DC;
    public const GLenum TEXTURE29 = 0x84DD;
    public const GLenum TEXTURE30 = 0x84DE;
    public const GLenum TEXTURE31 = 0x84DF;
    public const GLenum ACTIVE_TEXTURE = 0x84E0;

    public const GLenum BYTE = 0x1400;
    public const GLenum UNSIGNED_BYTE = 0x1401;
    public const GLenum SHORT = 0x1402;
    public const GLenum UNSIGNED_SHORT = 0x1403;
    public const GLenum INT = 0x1404;
    public const GLenum UNSIGNED_INT = 0x1405;
    public const GLenum FLOAT = 0x1406;
    public const GLenum FIXED = 0x140C;

    public const GLenum NEAREST = 0x2600;
    public const GLenum LINEAR = 0x2601;

    public const GLenum TEXTURE_MAG_FILTER = 0x2800;
    public const GLenum TEXTURE_MIN_FILTER = 0x2801;
    public const GLenum TEXTURE_WRAP_S = 0x2802;
    public const GLenum TEXTURE_WRAP_T = 0x2803;

    public const GLenum DEPTH_COMPONENT = 0x1902;
    public const GLenum ALPHA = 0x1906;
    public const GLenum RGB = 0x1907;
    public const GLenum RGBA = 0x1908;
    public const GLenum LUMINANCE = 0x1909;
    public const GLenum LUMINANCE_ALPHA = 0x190A;

    public const GLenum NEAREST_MIPMAP_NEAREST = 0x2700;
    public const GLenum LINEAR_MIPMAP_NEAREST = 0x2701;
    public const GLenum NEAREST_MIPMAP_LINEAR = 0x2702;
    public const GLenum LINEAR_MIPMAP_LINEAR = 0x2703;

    public const GLenum REPEAT = 0x2901;
    public const GLenum CLAMP_TO_EDGE = 0x812F;
    public const GLenum MIRRORED_REPEAT = 0x8370;

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
    public static partial void GetShaderInfoLog(GLuint shader, GLsizei maxLength, out GLsizei length,
        out string infoLog);

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
    public static partial void GetProgramInfoLog(GLuint shader, GLsizei maxLength, out GLsizei length,
        out string infoLog);

    [LibraryImport(dll, EntryPoint = "glDeleteProgram")]
    public static partial void DeleteProgram(GLuint program);

    [LibraryImport(dll, EntryPoint = "glGetActiveUniform")]
    public static partial void GetActiveUniform(GLuint program, GLuint index, GLsizei bufSize, out GLsizei length,
        out GLint size, out GLenum type, out string name);

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

    [LibraryImport(dll, EntryPoint = "glGenTextures")]
    public static partial void GenTextures(GLsizei n, out uint[] textures);

    [LibraryImport(dll, EntryPoint = "glActiveTexture")]
    public static partial void ActiveTexture(GLenum texture);

    [LibraryImport(dll, EntryPoint = "glBindTexture")]
    public static partial void BindTexture(GLenum target, GLuint texture);

    [LibraryImport(dll, EntryPoint = "glTexImage2D")]
    public static partial void TexImage2D(GLenum target, GLint level, GLint internalFormat, GLsizei width, GLsizei height, GLint border, GLenum format, GLenum type, ref dynamic data);
    
    [LibraryImport(dll, EntryPoint = "glTexParameter")]
    public static partial void TexParameter(GLenum target, GLenum pname, GLfloat param);

    [LibraryImport(dll, EntryPoint = "glTexParameter")]
    public static partial void TexParameter(GLenum target, GLenum pname, GLint param);

    [LibraryImport(dll, EntryPoint = "glTexParameter")]
    public static partial void TexParameter(GLenum target, GLenum pname, GLfloat[] _params);

    [LibraryImport(dll, EntryPoint = "glTexParameter")]
    public static partial void TexParameter(GLenum target, GLenum pname, GLint[] _params);

    [LibraryImport(dll, EntryPoint = "glGenerateMipmap")]
    public static partial void GenerateMipmap(GLenum target);
    
    [LibraryImport(dll, EntryPoint = "glDeleteTextures")]
    public static partial void DeleteTextures(GLsizei n, GLuint[] textures);
}