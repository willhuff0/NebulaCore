using System.Numerics;
using System.Text.Json.Nodes;
using NebulaCore.Angle;

namespace NebulaCore.Engine.Assets;

[AssetDefinition("renderShaders")]
public class RenderShader : FileAsset
{
    private string[] _imports;

    public RenderShader(Project project, JsonNode json) : base(project, json)
    {
        _imports = json["imports"]?.GetValue<string[]>() ?? Array.Empty<string>();
    }

    public override JsonObject Serialize()
    {
        var json = base.Serialize();
        json["imports"] = JsonValue.Create(_imports);
        return json;
    }

    public override Task<RuntimeAsset?> Load()
    {
        var content = FileAssetGetText();
        var lines = content.Split('\n');

        string preamble = "", vertexSource = "", fragmentSource = "";
        int stage = 0;
        foreach (var line in lines)
        {
            if (line.StartsWith("#VERTEX"))
            {
                stage = 1;
                continue;
            }
            if (line.StartsWith("#FRAGMENT"))
            {
                stage = 2;
                continue;
            }
            
            switch (stage)
            {
                case 0: 
                    preamble += line + '\n';
                    break;
                case 1:
                    vertexSource += line + '\n';
                    break;
                case 2:
                    fragmentSource += line + '\n';
                    break;
            }
        }

        for (int i = _imports.Length - 1; i >= 0; i--)
        {
            var library = LibraryShader.FindById(_imports[i]);
            if (library != null)
            {
                vertexSource = vertexSource.Insert(0, library.Content + '\n');
                fragmentSource = fragmentSource.Insert(0, library.Content + '\n');
            }
        }

        vertexSource = vertexSource.Insert(0, preamble + '\n');
        fragmentSource = fragmentSource.Insert(0, preamble + '\n');
        
        var vertexShader = GL.CreateShader(GL.VERTEX_SHADER);
        GL.ShaderSource(vertexShader, 1, new string[] { vertexSource }, null);
        GL.CompileShader(vertexShader);
        GL.GetShader(vertexShader, GL.COMPILE_STATUS, out int status);
        if (status == GL.FALSE)
        {
            GL.GetShaderInfoLog(vertexShader, 1024, out int length, out string infoLog);
            GL.DeleteShader(vertexShader);
            Console.WriteLine($"Error compiling vertex shader ({name}): {infoLog}");
            return Task.FromResult<RuntimeAsset?>(null);
        }
        
        var fragmentShader = GL.CreateShader(GL.FRAGMENT_SHADER);
        GL.ShaderSource(fragmentShader, 1, new string[] { fragmentSource }, null);
        GL.CompileShader(fragmentShader);
        GL.GetShader(fragmentShader, GL.COMPILE_STATUS, out status);
        if (status == GL.FALSE)
        {
            GL.GetShaderInfoLog(fragmentShader, 1024, out int length, out string infoLog);
            GL.DeleteShader(fragmentShader);
            Console.WriteLine($"Error compiling fragment shader ({name}): {infoLog}");
            return Task.FromResult<RuntimeAsset?>(null);
        }

        var program = GL.CreateProgram();
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        GL.LinkProgram(program);
        GL.DetachShader(program, vertexShader);
        GL.DetachShader(program, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
        GL.GetProgram(program, GL.LINK_STATUS, out status);
        if (status == GL.FALSE)
        {
            GL.GetProgramInfoLog(program, 1024, out int length, out string infoLog);
            GL.DeleteProgram(program);
            Console.WriteLine($"Error linking program ({name}): {infoLog}");
            return Task.FromResult<RuntimeAsset?>(null);
        }

        GL.GetProgram(program, GL.ACTIVE_UNIFORMS, out int uniformCount);
        Dictionary<string, int> uniformLocations = new Dictionary<string, int>(uniformCount);
        for (uint i = 0; i < uniformCount; i++)
        {
            GL.GetActiveUniform(program, i, 1024, out int length, out int size, out uint type, out string uniformName);
            uniformLocations[uniformName] = GL.GetUniformLocation(program, uniformName);
        }

        return Task.FromResult<RuntimeAsset?>(new RuntimeRenderShader(Project, program, uniformLocations));
    }
}

public class RuntimeRenderShader : RuntimeAsset
{
    private readonly uint _program;
    private readonly Dictionary<string, int> _uniformLocations;

    public RuntimeRenderShader(Project project, uint program, Dictionary<string, int> uniformLocations) : base(project)
    {
        _program = program;
        _uniformLocations = uniformLocations;
    }
    
    public override Task Unload()
    {
        GL.DeleteProgram(_program);
        
        return Task.CompletedTask;
    }

    public void Bind() => GL.UseProgram(_program);
    
    public void SetUniform(string name, float value) => GL.ProgramUniform(_program, _uniformLocations[name], value);
    public void SetUniform(string name, Vector2 value) => GL.ProgramUniform(_program, _uniformLocations[name], value.X, value.Y);
    public void SetUniform(string name, Vector3 value) => GL.ProgramUniform(_program, _uniformLocations[name], value.X, value.Y, value.Z);
    public void SetUniform(string name, Vector4 value) => GL.ProgramUniform(_program, _uniformLocations[name], value.X, value.Y, value.Z, value.W);
    public void SetUniform(string name, int value) => GL.ProgramUniform(_program, _uniformLocations[name], value);
    public void SetUniform(string name, int v0, int v1) => GL.ProgramUniform(_program, _uniformLocations[name], v0, v1);
    public void SetUniform(string name, int v0, int v1, int v2) => GL.ProgramUniform(_program, _uniformLocations[name], v0, v1, v2);
    public void SetUniform(string name, int v0, int v1, int v2, int v3) => GL.ProgramUniform(_program, _uniformLocations[name], v0, v1, v2, v3);
    public void SetUniform(string name, bool value) => GL.ProgramUniform(_program, _uniformLocations[name], value ? 1 : 0);
    public void SetUniform(string name, Matrix4x4 value) => GL.ProgramUniformMatrix4(_program, _uniformLocations[name], 1, false, new []{ value.M11, value.M21, value.M31, value.M41, value.M12, value.M22, value.M32, value.M42, value.M13, value.M23, value.M33, value.M43, value.M14, value.M24, value.M34, value.M44 });

    public void SetUniform(string name, object value)
    {
        switch (value)
        {
            case int val: 
                SetUniform(name, val);
                break;
            case bool val:
                SetUniform(name, val);
                break;
            case float val:
                SetUniform(name, val);
                break;
            case Vector2 val:
                SetUniform(name, val);
                break;
            case Vector3 val:
                SetUniform(name, val);
                break;
            case Vector4 val:
                SetUniform(name, val);
                break;
            case int[] val:
                switch (val.Length)
                {
                    case 1:
                        SetUniform(name, val[0]);
                        break;
                    case 2:
                        SetUniform(name, val[0], val[1]);
                        break;
                    case 3:
                        SetUniform(name, val[0], val[1], val[2]);
                        break;
                    case 4:
                        SetUniform(name, val[0], val[1], val[2], val[3]);
                        break;
                }
                break;
            case float[] val:
                switch (val.Length)
                {
                    case 1:
                        SetUniform(name, val[0]);
                        break;
                    case 2:
                        SetUniform(name, new Vector2(val[0], val[1]));
                        break;
                    case 3:
                        SetUniform(name, new Vector3(val[0], val[1], val[2]));
                        break;
                    case 4:
                        SetUniform(name, new Vector4(val[0], val[1], val[2], val[3]));
                        break;
                    case 16:
                        SetUniform(name, new Matrix4x4(val[0], val[1], val[2], val[3], val[4], val[5], val[6], val[7], val[8], val[9], val[10], val[11], val[12], val[13], val[14], val[15]));
                        break;
                }
                break;
        }
    }
}