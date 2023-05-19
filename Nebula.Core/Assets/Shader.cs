using Nebula.Graphics;
using Nebula.Math;

namespace Nebula.Core.Assets;

[AssetLoader("shader")]
public class Shader : Asset, IAssetLoader
{
    public static async Task<Asset?> LoadAssetAsync(Project project, AssetDefinition definition)
    {
        try
        {
            var binaryPath = Path.Join(project.AppDirectory, "shaders", "binaries", definition.Key.ToString());
            var formatPath = $"{binaryPath}.format";
            if (!File.Exists(binaryPath))
            {
                var vertPath = Path.Join(project.AppDirectory, "shaders", "sources", definition.Key.ToString(), "vertex.glsl");
                var fragPath = Path.Join(project.AppDirectory, "shaders", "sources", definition.Key.ToString(), "fragment.glsl");
                
                var vertSource = await File.ReadAllTextAsync(vertPath);
                var fragSource = await File.ReadAllTextAsync(fragPath);

                var vertShader = GL.CreateShader(GL.VERTEX_SHADER);
                GL.ShaderSource(vertShader, 1, new string[] { vertSource }, new []{vertSource.Length});
                GL.CompileShader(vertShader);
                GL.GetShader(vertShader, GL.COMPILE_STATUS, out int status);
                if (status == GL.FALSE)
                {
                    GL.GetShaderInfoLog(vertShader, 1024, out int length, out string infoLog);
                    GL.DeleteShader(vertShader);
                    Console.WriteLine($"Error compiling vertex shader ({definition.Key}): {infoLog}");
                    return null;
                }
        
                var fragShader = GL.CreateShader(GL.FRAGMENT_SHADER);
                GL.ShaderSource(fragShader, 1, new string[] { fragSource }, new []{fragSource.Length});
                GL.CompileShader(fragShader);
                GL.GetShader(fragShader, GL.COMPILE_STATUS, out status);
                if (status == GL.FALSE)
                {
                    GL.GetShaderInfoLog(fragShader, 1024, out int length, out string infoLog);
                    GL.DeleteShader(fragShader);
                    Console.WriteLine($"Error compiling fragment shader ({definition.Key}): {infoLog}");
                    return null;
                }
                
                var program = GL.CreateProgram();
                GL.AttachShader(program, vertShader);
                GL.AttachShader(program, fragShader);
                GL.LinkProgram(program);
                GL.DetachShader(program, vertShader);
                GL.DetachShader(program, fragShader);
                GL.DeleteShader(vertShader);
                GL.DeleteShader(fragShader);
                GL.GetProgram(program, GL.LINK_STATUS, out status);
                if (status == GL.FALSE)
                {
                    GL.GetProgramInfoLog(program, 1024, out int length, out string infoLog);
                    GL.DeleteProgram(program);
                    Console.WriteLine($"Error linking program ({definition.Key}): {infoLog}");
                    return null;
                }
                
                GL.GetProgram(program, GL.PROGRAM_BINARY_LENGTH, out int binaryLength);
                GL.GetProgramBinary(program, binaryLength, out int _, out uint binaryFormat, out byte[] binary);
                await File.WriteAllBytesAsync(binaryPath, binary);
                await File.WriteAllTextAsync(formatPath, binaryFormat.ToString());
                
                GL.DeleteProgram(program);
                
                return new Shader(definition.Type, definition.Key, binaryPath, binaryFormat);
            }

            var format = uint.Parse(await File.ReadAllTextAsync(formatPath));
            return new Shader(definition.Type, definition.Key, binaryPath, format);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading shader asset ({definition.Key}): {e}");
            return null;
        }
    }

    private readonly string _binaryPath;
    private readonly uint _binaryFormat;

    private Shader(string type, Guid key, string binaryPath, uint binaryFormat) : base(type, key)
    {
        _binaryPath = binaryPath;
        _binaryFormat = binaryFormat;
    }

    private uint _program;

    public override void EnterScope()
    {
        var binary = File.ReadAllBytes(_binaryPath);
        
        _program = GL.CreateProgram();
        GL.ProgramBinary(_program, _binaryFormat, binary, binary.Length);
        
        GL.GetProgram(_program, GL.LINK_STATUS, out int status);
        if (status == GL.FALSE)
        {
            GL.GetProgramInfoLog(_program, 1024, out int length, out string infoLog);
            GL.DeleteProgram(_program);
            Console.WriteLine($"Error linking program ({Key}): {infoLog}");
        }
    }

    public override void ExitScope()
    {
        GL.DeleteProgram(_program);
        _program = 0;
    }
    
    public void Bind() => GL.UseProgram(_program);
    
    public void SetUniform(int location, float value) => GL.ProgramUniform(_program, location, value);
    public void SetUniform(int location, Vector2<float> value) => GL.ProgramUniform(_program, location, value.X, value.Y);
    public void SetUniform(int location, Vector3<float> value) => GL.ProgramUniform(_program, location, value.X, value.Y, value.Z);
    public void SetUniform(int location, Vector4<float> value) => GL.ProgramUniform(_program, location, value.X, value.Y, value.Z, value.W);
    public void SetUniform(int location, int value) => GL.ProgramUniform(_program, location, value);
    public void SetUniform(int location, Vector2<int> value) => GL.ProgramUniform(_program, location, value.X, value.Y);
    public void SetUniform(int location, Vector3<int> value) => GL.ProgramUniform(_program, location, value.X, value.Y, value.Z);
    public void SetUniform(int location, Vector4<int> value) => GL.ProgramUniform(_program, location, value.X, value.Y, value.Z, value.W);
    public void SetUniform(int location, bool value) => GL.ProgramUniform(_program, location, value ? 1 : 0);
    public void SetUniform(int location, Matrix4<float> value) => GL.ProgramUniformMatrix4(_program, location, 1, false, value.ToArray());

    public void SetUniform(int location, object value)
    {
        switch (value)
        {
            case int val: 
                SetUniform(location, val);
                break;
            case bool val:
                SetUniform(location, val);
                break;
            case float val:
                SetUniform(location, val);
                break;
            case Vector2<float> val:
                SetUniform(location, val);
                break;
            case Vector3<float> val:
                SetUniform(location, val);
                break;
            case Vector4<float> val:
                SetUniform(location, val);
                break;
            case int[] val:
                switch (val.Length)
                {
                    case 1:
                        SetUniform(location, val[0]);
                        break;
                    case 2:
                        SetUniform(location, new Vector2<int>(val[0], val[1]));
                        break;
                    case 3:
                        SetUniform(location, new Vector3<int>(val[0], val[1], val[2]));
                        break;
                    case 4:
                        SetUniform(location, new Vector4<int>(val[0], val[1], val[2], val[3]));
                        break;
                }
                break;
            case float[] val:
                switch (val.Length)
                {
                    case 1:
                        SetUniform(location, val[0]);
                        break;
                    case 2:
                        SetUniform(location, new Vector2<float>(val[0], val[1]));
                        break;
                    case 3:
                        SetUniform(location, new Vector3<float>(val[0], val[1], val[2]));
                        break;
                    case 4:
                        SetUniform(location, new Vector4<float>(val[0], val[1], val[2], val[3]));
                        break;
                    case 16:
                        SetUniform(location, new Matrix4<float>(val[0], val[1], val[2], val[3], val[4], val[5], val[6], val[7], val[8], val[9], val[10], val[11], val[12], val[13], val[14], val[15]));
                        break;
                }
                break;
        }
    }
}