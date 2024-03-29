using System.Runtime.InteropServices;
using System.Text;

namespace Nebula.Graphics;

using GLuint = UInt32;
using GLenum = UInt32;
using GLubyte = Byte;
using GLfloat = Single;
using GLsizei = Int32;
using GLchar = Char;
using GLint = Int32;
using GLsizeiptr = Int64;

public static partial class GL
{
#if WINDOWS
    private const string glesDll = "libGLESv2.dll";
    private const string eglDll = "libEGL.dll";
#else
    private const string glesDll = "libGLESv2.dylib";
    private const string eglDll = "libEGL.dylib";
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

    public const GLenum ARRAY_BUFFER = 0x8892;
    public const GLenum ELEMENT_ARRAY_BUFFER = 0x8893;
    public const GLenum ARRAY_BUFFER_BINDING = 0x8894;
    public const GLenum ELEMENT_ARRAY_BUFFER_BINDING = 0x8895;
    public const GLenum STREAM_DRAW = 0x88E0;
    public const GLenum STATIC_DRAW = 0x88E4;
    public const GLenum DYNAMIC_DRAW = 0x88E8;
    public const GLenum BUFFER_SIZE = 0x8764;
    public const GLenum BUFFER_USAGE = 0x8765;
    public const GLenum CURRENT_VERTEX_ATTRIB = 0x8626;

    public const GLenum POINTS = 0x0000;
    public const GLenum LINES = 0x0001;
    public const GLenum LINE_LOOP = 0x0002;
    public const GLenum LINE_STRIP = 0x0003;
    public const GLenum TRIANGLES = 0x0004;
    public const GLenum TRIANGLE_STRIP = 0x0005;
    public const GLenum TRIANGLE_FAN = 0x0006;

    public const GLenum PROGRAM_BINARY_LENGTH = 0x8741;
    public const GLenum NUM_PROGRAM_BINARY_FORMATS = 0x87FE;
    public const GLenum PROGRAM_BINARY_FORMATS = 0x87FF;

    public const GLenum NO_ERROR = 0;
    public const GLenum INVALID_ENUM = 0x0500;
    public const GLenum INVALID_VALUE = 0x0501;
    public const GLenum INVALID_OPERATION = 0x0502;
    public const GLenum INVALID_FRAMEBUFFER_OPERATION = 0x0506;
    public const GLenum OUT_OF_MEMORY = 0x0505;
    
    [LibraryImport(glesDll, EntryPoint = "glGetBooleanv", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void Get(GLenum pname, [MarshalAs(UnmanagedType.U1)] out bool data);
    
    [LibraryImport(glesDll, EntryPoint = "glGetBooleanv", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void Get(GLenum pname, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] bool[] data);
    
    [LibraryImport(glesDll, EntryPoint = "glGetFloatv", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void Get(GLenum pname, out GLfloat data);
    
    [LibraryImport(glesDll, EntryPoint = "glGetFloatv", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void Get(GLenum pname, GLfloat[] data);
    
    [LibraryImport(glesDll, EntryPoint = "glGetIntegerv", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void Get(GLenum pname, out GLint data);
    
    [LibraryImport(glesDll, EntryPoint = "glGetIntegerv", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void Get(GLenum pname, GLint[] data);
    
    [LibraryImport(glesDll, EntryPoint = "glGetInteger64v", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void Get(GLenum pname, out Int64 data);
    
    [LibraryImport(glesDll, EntryPoint = "glGetInteger64v", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void Get(GLenum pname, Int64[] data);
    
    [LibraryImport(glesDll, EntryPoint = "glGetBooleani_v", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void Get(GLenum target, GLuint index, [MarshalAs(UnmanagedType.U1)] out bool data);
    
    [LibraryImport(glesDll, EntryPoint = "glGetIntegeri_v", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void Get(GLenum target, GLuint index, out GLint data);
    
    [LibraryImport(glesDll, EntryPoint = "glGetInteger64i_v", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void Get(GLenum target, GLuint index, out Int64 data);

    [LibraryImport(glesDll, EntryPoint = "glGetString", StringMarshalling = StringMarshalling.Utf8)]
    public static partial string GetString(GLenum name);
    
    [LibraryImport(glesDll, EntryPoint = "glGetError", StringMarshalling = StringMarshalling.Utf8)]
    public static partial GLenum GetError();

    public static string GetErrorDescription(GLenum error)
    {
        switch (error)
        {
            case NO_ERROR:
                return "GL_NO_ERROR: No error has been recorded.";
            case INVALID_ENUM:
                return "GL_INVALID_ENUM: An unacceptable value is specified for an enumerated argument.";
            case INVALID_VALUE:
                return "GL_INVALID_VALUE: A numeric argument is out of range.";
            case INVALID_OPERATION:
                return "GL_INVALID_OPERATION: The specified operation is not allowed in the current state.";
            case INVALID_FRAMEBUFFER_OPERATION:
                return "GL_INVALID_FRAMEBUFFER_OPERATION: The framebuffer object is not complete.";
            case OUT_OF_MEMORY:
                return "GL_OUT_OF_MEMORY: There is not enough memory left to execute the command. The state of the GL is undefined, except for the state of the error flags, after this error is recorded.";
            default:
                return $"{error} is not a valid GLES error code.";
        }
    }

    public static void CheckError()
    {
        var error = GetError();
        if (error != NO_ERROR) throw new Exception($"OpenGL ES error {error}, {GetErrorDescription(error)}");
    }

    [LibraryImport(glesDll, EntryPoint = "glClearColor", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void ClearColor(GLfloat red, GLfloat green, GLfloat blue, GLfloat alpha);

    [LibraryImport(glesDll, EntryPoint = "glClear", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void Clear(GLenum mask);

    [LibraryImport(glesDll, EntryPoint = "glEnable", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void Enable(GLenum cap);

    [LibraryImport(glesDll, EntryPoint = "glDisable", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void Disable(GLenum cap);

    [LibraryImport(glesDll, EntryPoint = "glCullFace", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void CullFace(GLenum mode);

    [LibraryImport(glesDll, EntryPoint = "glViewport", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void Viewport(int x, int y, GLsizei width, GLsizei height);

    [LibraryImport(glesDll, EntryPoint = "glCreateShader", StringMarshalling = StringMarshalling.Utf8)]
    public static partial GLuint CreateShader(GLenum shaderType);

    [LibraryImport(glesDll, EntryPoint = "glShaderSource", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void ShaderSource(GLuint shader, GLsizei count, string[] source, GLint[]? length);

    [LibraryImport(glesDll, EntryPoint = "glGetShaderSource", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void GetShaderSource(GLuint shader, GLsizei bufSize, out GLsizei length, out string source);

    // [LibraryImport(dll, EntryPoint = "glShaderSource")]
    // public static partial void ShaderSource(GLuint shader, GLsizei count, string source, GLint[]? length = null);

    [LibraryImport(glesDll, EntryPoint = "glCompileShader", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void CompileShader(GLuint shader);

    [LibraryImport(glesDll, EntryPoint = "glGetShaderiv", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void GetShader(GLuint shader, GLenum pname, out GLint param);

    [LibraryImport(glesDll, EntryPoint = "glGetShaderInfoLog", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void GetShaderInfoLog(GLuint shader, GLsizei maxLength, out GLsizei length, byte[] infoLog);

    public static string GetShaderInfoLog(GLuint shader)
    {
        GetShader(shader, INFO_LOG_LENGTH, out var maxLength);
        var infoLog = new byte[maxLength];
        GetShaderInfoLog(shader, maxLength, out var length, infoLog);
        return Encoding.UTF8.GetString(infoLog, 0, length);
    }

    [LibraryImport(glesDll, EntryPoint = "glCreateProgram", StringMarshalling = StringMarshalling.Utf8)]
    public static partial GLuint CreateProgram();

    [LibraryImport(glesDll, EntryPoint = "glAttachShader", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void AttachShader(GLuint program, GLuint shader);

    [LibraryImport(glesDll, EntryPoint = "glLinkProgram", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void LinkProgram(GLuint program);

    [LibraryImport(glesDll, EntryPoint = "glDetachShader", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void DetachShader(GLuint program, GLuint shader);

    [LibraryImport(glesDll, EntryPoint = "glDeleteShader", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void DeleteShader(GLuint shader);

    [LibraryImport(glesDll, EntryPoint = "glGetProgramiv", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void GetProgram(GLuint program, GLenum pname, out GLint param);

    [LibraryImport(glesDll, EntryPoint = "glGetProgramInfoLog", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void GetProgramInfoLog(GLuint program, GLsizei maxLength, out GLsizei length, byte[] infoLog);
    
    public static string GetProgramInfoLog(GLuint program)
    {
        GetProgram(program, INFO_LOG_LENGTH, out var maxLength);
        var infoLog = new byte[maxLength];
        GetProgramInfoLog(program, maxLength, out var length, infoLog);
        return Encoding.UTF8.GetString(infoLog, 0, length);
    }

    [LibraryImport(glesDll, EntryPoint = "glDeleteProgram", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void DeleteProgram(GLuint program);

    [LibraryImport(glesDll, EntryPoint = "glGetProgramBinary", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void GetProgramBinary(GLuint program, GLsizei bufsize, out GLsizei length, out GLenum binaryFormat, byte[] binary);

    public static byte[] GetProgramBinary(GLuint program, out GLenum binaryFormat)
    {
        GetProgram(program, PROGRAM_BINARY_LENGTH, out var maxLength);
        var binary = new byte[maxLength];
        GetProgramBinary(program, maxLength, out var length, out binaryFormat, binary);
        return binary.Take(length).ToArray();
    }

    [LibraryImport(glesDll, EntryPoint = "glProgramBinary", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void ProgramBinary(GLuint program, GLenum binaryFormat, [MarshalAs(UnmanagedType.LPArray)] in byte[] binary, GLsizei length);
    
    [LibraryImport(glesDll, EntryPoint = "glGetActiveUniform", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void GetActiveUniform(GLuint program, GLuint index, GLsizei bufSize, out GLsizei length, out GLint size, out GLenum type, out string name);

    [LibraryImport(glesDll, EntryPoint = "glGetUniformLocation", StringMarshalling = StringMarshalling.Utf8)]
    public static partial GLint GetUniformLocation(GLuint program, string name);

    [LibraryImport(glesDll, EntryPoint = "glUseProgram", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void UseProgram(GLuint program);

    [LibraryImport(glesDll, EntryPoint = "glProgramUniform1f", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void ProgramUniform(GLuint program, GLint location, GLfloat v0);

    [LibraryImport(glesDll, EntryPoint = "glProgramUniform2f", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void ProgramUniform(GLuint program, GLint location, GLfloat v0, GLfloat v1);

    [LibraryImport(glesDll, EntryPoint = "glProgramUniform3f", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void ProgramUniform(GLuint program, GLint location, GLfloat v0, GLfloat v1, GLfloat v2);

    [LibraryImport(glesDll, EntryPoint = "glProgramUniform4f", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void ProgramUniform(GLuint program, GLint location, GLfloat v0, GLfloat v1, GLfloat v2, GLfloat v3);

    [LibraryImport(glesDll, EntryPoint = "glProgramUniform1i", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void ProgramUniform(GLuint program, GLint location, GLint v0);

    [LibraryImport(glesDll, EntryPoint = "glProgramUniform2i", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void ProgramUniform(GLuint program, GLint location, GLint v0, GLint v1);

    [LibraryImport(glesDll, EntryPoint = "glProgramUniform3i", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void ProgramUniform(GLuint program, GLint location, GLint v0, GLint v1, GLint v2);

    [LibraryImport(glesDll, EntryPoint = "glProgramUniform4i", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void ProgramUniform(GLuint program, GLint location, GLint v0, GLint v1, GLint v2, GLint v3);

    [LibraryImport(glesDll, EntryPoint = "glProgramUniform1ui", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void ProgramUniform(GLuint program, GLint location, GLuint v0);

    [LibraryImport(glesDll, EntryPoint = "glProgramUniform2ui", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void ProgramUniform(GLuint program, GLint location, GLuint v0, GLuint v1);

    [LibraryImport(glesDll, EntryPoint = "glProgramUniform3ui", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void ProgramUniform(GLuint program, GLint location, GLuint v0, GLuint v1, GLuint v2);

    [LibraryImport(glesDll, EntryPoint = "glProgramUniform4ui", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void ProgramUniform(GLuint program, GLint location, GLuint v0, GLuint v1, GLuint v2, GLuint v3);

    [LibraryImport(glesDll, EntryPoint = "glProgramUniformMatrix4fv", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void ProgramUniformMatrix4(GLuint program, GLint location, GLsizei count, [MarshalAs(UnmanagedType.U1)] bool transpose, float[] value);

    [LibraryImport(glesDll, EntryPoint = "glGenTextures", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void GenTextures(GLsizei n, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4, SizeParamIndex = 0)] out GLuint[] textures);

    [LibraryImport(glesDll, EntryPoint = "glActiveTexture", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void ActiveTexture(GLenum texture);

    [LibraryImport(glesDll, EntryPoint = "glBindTexture", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void BindTexture(GLenum target, GLuint texture);

    [LibraryImport(glesDll, EntryPoint = "glTexImage2D", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void TexImage2D(GLenum target, GLint level, GLint internalFormat, GLsizei width, GLsizei height, GLint border, GLenum format, GLenum type, [MarshalAs(UnmanagedType.LPArray)] in byte[] data);

    [LibraryImport(glesDll, EntryPoint = "glCompressedTexImage2D", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void CompressedTexImage2D(GLenum target, GLint level, GLenum internalFormat, GLsizei width, GLsizei height, GLint border, GLsizei imageSize, [MarshalAs(UnmanagedType.LPArray)] in byte[] data);
    
    [LibraryImport(glesDll, EntryPoint = "glTexParameter", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void TexParameter(GLenum target, GLenum pname, GLfloat param);

    [LibraryImport(glesDll, EntryPoint = "glTexParameter", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void TexParameter(GLenum target, GLenum pname, GLint param);

    [LibraryImport(glesDll, EntryPoint = "glTexParameter", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void TexParameter(GLenum target, GLenum pname, GLfloat[] _params);

    [LibraryImport(glesDll, EntryPoint = "glTexParameter", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void TexParameter(GLenum target, GLenum pname, GLint[] _params);

    [LibraryImport(glesDll, EntryPoint = "glGenerateMipmap", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void GenerateMipmap(GLenum target);
    
    [LibraryImport(glesDll, EntryPoint = "glDeleteTextures", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void DeleteTextures(GLsizei n, GLuint[] textures);

    [LibraryImport(glesDll, EntryPoint = "glGenVertexArrays", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void GenVertexArrays(GLsizei n, GLuint[] arrays);

    public static GLuint[] GenVertexArrays(GLsizei n)
    {
        var vertexArrays = new uint[n];
        GenVertexArrays(n, vertexArrays);
        return vertexArrays;
    }
    
    public static GLuint GenVertexArray() => GenVertexArrays(1)[0];

    [LibraryImport(glesDll, EntryPoint = "glBindVertexArray", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void BindVertexArray(GLuint array);

    [LibraryImport(glesDll, EntryPoint = "glDeleteVertexArrays", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void DeleteVertexArrays(GLsizei n, GLuint[] arrays);

    [LibraryImport(glesDll, EntryPoint = "glGenBuffers", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void GenBuffers(GLsizei n, GLuint[] buffers);
    
    public static GLuint[] GenBuffers(GLsizei n)
    {
        var buffers = new uint[n];
        GenVertexArrays(n, buffers);
        return buffers;
    }
    
    public static GLuint GenBuffer() => GenBuffers(1)[0];
    
    [LibraryImport(glesDll, EntryPoint = "glDeleteBuffers", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void DeleteBuffers(GLsizei n, GLuint[] buffers);

    [LibraryImport(glesDll, EntryPoint = "glBindBuffer", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void BindBuffer(GLenum target, GLuint buffer);

    [LibraryImport(glesDll, EntryPoint = "glBufferData", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void BufferData(GLenum target, GLsizeiptr size, byte[] data, GLenum usage);

    [LibraryImport(glesDll, EntryPoint = "glEnableVertexAttribArray", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void EnableVertexAttribArray(GLuint index);
    
    [LibraryImport(glesDll, EntryPoint = "glDisableVertexAttribArray", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void DisableVertexAttribArray(GLuint index);
    
    [LibraryImport(glesDll, EntryPoint = "glVertexAttribPointer", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void VertexAttribPointer(GLuint index, GLint size, GLenum type, [MarshalAs(UnmanagedType.U1)] bool normalized, GLsizei stride, IntPtr pointer);

    [LibraryImport(glesDll, EntryPoint = "glVertexAttribPointer", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void VertexAttribPointer(GLuint index, GLint size, GLenum type, GLsizei stride, IntPtr pointer);

    [LibraryImport(glesDll, EntryPoint = "glDrawElements", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void DrawElements(GLenum mode, GLsizei count, GLenum type, IntPtr indices);

    [LibraryImport(glesDll, EntryPoint = "glDrawArrays", StringMarshalling = StringMarshalling.Utf8)]
    public static partial void DrawArrays(GLenum mode, GLint first, GLsizei count);
}