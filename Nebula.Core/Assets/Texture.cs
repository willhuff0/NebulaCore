using Nebula.Graphics;

namespace Nebula.Core.Assets;

[AssetLoader("texture")]
public class Texture : Asset, IAssetLoader
{
    public static Task<Asset?> LoadAssetAsync(Project project, AssetDefinition definition)
    {
        try
        {
            var path = Path.Join(project.AppDirectory, "textures", definition.Key.ToString());
            if (!File.Exists(path))
            {
                Console.WriteLine($"Error loading texture asset ({definition.Key}): Data file at \"${path}\" does not exist.");
                return Task.FromResult<Asset?>(null);
            }

            var format = definition.Properties?["format"]?.GetValue<uint?>() ?? 0x93B0; // COMPRESSED_RGBA_ASTC_4x4_KHR
            var width = definition.Properties!["width"]!.GetValue<int>();
            var height = definition.Properties!["height"]!.GetValue<int>();
            
            return Task.FromResult<Asset?>(new Texture(definition.Type, definition.Key, path, format, width, height));
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading texture asset ({definition.Key}): {e}");
            return Task.FromResult<Asset?>(null);
        }
    }

    private readonly string _path;
    private readonly uint _internalFormat;
    private readonly int _width;
    private readonly int _height;

    private Texture(string type, Guid key, string path, uint internalFormat, int width, int height) : base(type, key)
    {
        _path = path;
        _internalFormat = internalFormat;
        _width = width;
        _height = height;
    }

    private uint _texture;

    public override void EnterScope()
    {
        var data = File.ReadAllBytes(_path);
        
        GL.GenTextures(1, out var textures);
        _texture = textures[0];
        GL.ActiveTexture(GL.TEXTURE0);
        GL.BindTexture(GL.TEXTURE_2D, _texture);

        GL.CompressedTexImage2D(GL.TEXTURE_2D, 0, _internalFormat, _width, _height, 0, data.Length, data);
        
        GL.TexParameter(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.LINEAR);
        GL.TexParameter(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, GL.LINEAR);
        GL.TexParameter(GL.TEXTURE_2D, GL.TEXTURE_WRAP_S, GL.REPEAT);
        GL.TexParameter(GL.TEXTURE_2D, GL.TEXTURE_WRAP_T, GL.REPEAT);
        
        GL.GenerateMipmap(GL.TEXTURE_2D);
        
        GL.BindTexture(GL.TEXTURE_2D, 0);
    }

    public override void ExitScope()
    {
        GL.DeleteTextures(1, new []{_texture});
        _texture = 0;
    }
    
    public void Bind(int unit)
    {
        GL.ActiveTexture(TextureUnit[unit]);
        GL.BindTexture(GL.TEXTURE_2D, _texture);
    }
    
    private static readonly Dictionary<int, uint> TextureUnit = new() 
    {
        { 0, GL.TEXTURE0 },
        { 1, GL.TEXTURE1 },
        { 2, GL.TEXTURE2 },
        { 3, GL.TEXTURE3 },
        { 4, GL.TEXTURE4 },
        { 5, GL.TEXTURE5 },
        { 6, GL.TEXTURE6 },
        { 7, GL.TEXTURE7 },
        { 8, GL.TEXTURE8 },
        { 9, GL.TEXTURE9 },
        { 10, GL.TEXTURE10 },
        { 11, GL.TEXTURE11 },
        { 12, GL.TEXTURE12 },
        { 13, GL.TEXTURE13 },
        { 14, GL.TEXTURE14 },
        { 15, GL.TEXTURE15 },
        { 16, GL.TEXTURE16 },
        { 17, GL.TEXTURE17 },
        { 18, GL.TEXTURE18 },
        { 19, GL.TEXTURE19 },
        { 20, GL.TEXTURE20 },
        { 21, GL.TEXTURE21 },
        { 22, GL.TEXTURE22 },
        { 23, GL.TEXTURE23 },
        { 24, GL.TEXTURE24 },
        { 25, GL.TEXTURE25 },
        { 26, GL.TEXTURE26 },
        { 27, GL.TEXTURE27 },
        { 28, GL.TEXTURE28 },
        { 29, GL.TEXTURE29 },
        { 30, GL.TEXTURE30 },
        { 31, GL.TEXTURE31 },
    };
}