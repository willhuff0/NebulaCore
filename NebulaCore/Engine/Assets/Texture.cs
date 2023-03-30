using System.Text.Json.Nodes;
using NebulaCore.Angle;
using StbImageSharp;

namespace NebulaCore.Engine.Assets;

[AssetDefinition("textures")]
public class Texture : FileAsset
{
    public Texture(Project project, JsonNode json) : base(project, json)
    {
        
    }
    
    public override JsonObject Serialize()
    {
        var json = base.Serialize();
        return json;
    }

    public override Task<RuntimeAsset?> Load()
    {
        var fileStream = File.OpenRead(Path.Join(Project.Root, path));
        StbImage.stbi_set_flip_vertically_on_load(1);
        var image = ImageResult.FromStream(fileStream, ColorComponents.RedGreenBlueAlpha);
        dynamic data = image.Data;
        fileStream.Close();

        GL.GenTextures(1, out var textures);
        var texture = textures[0];
        GL.ActiveTexture(GL.TEXTURE0);
        GL.BindTexture(GL.TEXTURE_2D, texture);

        GL.TexImage2D(GL.TEXTURE_2D, 0, (int)GL.RGBA, image.Width, image.Height, 0, GL.RGBA, GL.UNSIGNED_BYTE, ref data);
        
        GL.TexParameter(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.LINEAR);
        GL.TexParameter(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, GL.LINEAR);
        GL.TexParameter(GL.TEXTURE_2D, GL.TEXTURE_WRAP_S, GL.REPEAT);
        GL.TexParameter(GL.TEXTURE_2D, GL.TEXTURE_WRAP_T, GL.REPEAT);
        
        GL.GenerateMipmap(GL.TEXTURE_2D);
        
        GL.BindTexture(GL.TEXTURE_2D, 0);

        return Task.FromResult<RuntimeAsset?>(new RuntimeTexture(Project, texture));
    }
}

public class RuntimeTexture : RuntimeAsset
{
    private uint _texture;
    
    public RuntimeTexture(Project project, uint texture) : base(project)
    {
        _texture = texture;
    }

    public override Task Unload()
    {
        GL.DeleteTextures(1, new []{_texture});
        return Task.CompletedTask;
    }

    private static readonly Dictionary<int, uint> _textureUnitLookup = new Dictionary<int, uint>()
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

    public void Bind(int slot)
    {
        GL.ActiveTexture(_textureUnitLookup[slot]);
        GL.BindTexture(GL.TEXTURE_2D, _texture);
    }
}