using Nebula.Core;
using Nebula.Graphics;
using Nebula.Math;

namespace Nebula.BuiltIn.Assets;

[AssetLoader("shader")]
public class Shader : Asset, IAssetLoader
{
    public static Asset? LoadAsset(Project project, AssetDefinition definition, bool immediateEnterScope)
    {
        try
        {
            var binaryPath = Path.Join(project.AppDirectory, "shaders", "binaries", definition.Key.ToString());
            var formatPath = $"{binaryPath}.format";
            if (File.Exists(binaryPath))
            {
                var binaryFormat = uint.Parse(File.ReadAllText(formatPath));
                var shader = new Shader(project.Database, definition.Type, definition.Key, binaryPath, binaryFormat);
                if (immediateEnterScope) shader.EnterScope();
                return shader;
            }
            else
            {
                var vertPath = Path.Join(project.AppDirectory, "shaders", "sources", definition.Key.ToString(), "vertex.glsl");
                var fragPath = Path.Join(project.AppDirectory, "shaders", "sources", definition.Key.ToString(), "fragment.glsl");
                
                var binary = _generateProgramBinary(vertPath, fragPath, definition.Key, binaryPath, formatPath, keepProgram: immediateEnterScope);
                if (binary == null) return null;
                
                var shader = new Shader(project.Database, definition.Type, definition.Key, binaryPath, binary.Value.binaryFormat);
                if (immediateEnterScope)
                {
                    shader._program = (uint)binary.Value.program!;
                    shader.EnterScope();
                }
                return shader;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading shader asset ({definition.Key}): {e}");
            return null;
        }
    }

    private static (uint binaryFormat, uint? program)? _generateProgramBinary(string vertPath, string fragPath, Guid assetKey, string binaryPath, string formatPath, bool keepProgram = false)
    {
        Console.WriteLine($"Generating program binary for shader {assetKey}");

        var vertSource = File.ReadAllText(vertPath);
        var fragSource = File.ReadAllText(fragPath);

        var vertShader = GL.CreateShader(GL.VERTEX_SHADER);
        GL.ShaderSource(vertShader, 1, new [] { vertSource }, new []{ vertSource.Length });
        GL.CompileShader(vertShader);
        GL.GetShader(vertShader, GL.COMPILE_STATUS, out int status);
        if (status == GL.FALSE)
        {
            var infoLog = GL.GetShaderInfoLog(vertShader);
            GL.DeleteShader(vertShader);
            Console.WriteLine($"Error compiling vertex shader ({assetKey}):\n{infoLog}");
            return null;
        }

        var fragShader = GL.CreateShader(GL.FRAGMENT_SHADER);
        GL.ShaderSource(fragShader, 1, new [] { fragSource }, new []{ fragSource.Length });
        GL.CompileShader(fragShader);
        GL.GetShader(fragShader, GL.COMPILE_STATUS, out status);
        if (status == GL.FALSE)
        {
            var infoLog = GL.GetShaderInfoLog(fragShader);
            GL.DeleteShader(fragShader);
            Console.WriteLine($"Error compiling fragment shader ({assetKey}):\n{infoLog}");
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
            var infoLog = GL.GetProgramInfoLog(program);
            GL.DeleteProgram(program);
            Console.WriteLine($"Error linking program ({assetKey}): {infoLog}");
            return null;
        }
        
        var binary = GL.GetProgramBinary(program, out var binaryFormat);
        Directory.CreateDirectory(Path.GetDirectoryName(binaryPath)!);
        File.WriteAllBytes(binaryPath, binary);
        File.WriteAllText(formatPath, binaryFormat.ToString());

        if (keepProgram) return (binaryFormat, program);
        GL.DeleteProgram(program);
        return (binaryFormat, null);
    }

    private void _regenerateProgramBinary()
    {
        var formatPath = $"{_binaryPath}.format";
        var vertPath = Path.Join(Database.AssociatedProject.AppDirectory, "shaders", "sources", Key.ToString(), "vertex.glsl");
        var fragPath = Path.Join(Database.AssociatedProject.AppDirectory, "shaders", "sources", Key.ToString(), "fragment.glsl");

        var binary = _generateProgramBinary(vertPath, fragPath, Key, _binaryPath, formatPath, keepProgram: true);
        if (binary == null) return;
        
        _binaryFormat = binary.Value.binaryFormat;
        _program = (uint)binary.Value.program!;
    }

    private readonly string _binaryPath;
    private uint _binaryFormat;

    private Shader(AssetDatabase database, string type, Guid key, string binaryPath, uint binaryFormat) : base(database, type, key)
    {
        _binaryPath = binaryPath;
        _binaryFormat = binaryFormat;
    }

    private uint _program;

    protected override void EnterScope()
    {
        if (_program == 0)
        {
            var data = File.ReadAllBytes(_binaryPath);

            _program = GL.CreateProgram();
            GL.ProgramBinary(_program, _binaryFormat, data, data.Length);

            GL.GetProgram(_program, GL.LINK_STATUS, out int status);
            if (status == GL.FALSE)
            {
                var infoLog = GL.GetProgramInfoLog(_program);
                GL.DeleteProgram(_program);
                Console.WriteLine($"Error linking program ({Key}): {infoLog}");

                _program = 0;
                _regenerateProgramBinary();
            }
        }
    }

    protected override void ExitScope()
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