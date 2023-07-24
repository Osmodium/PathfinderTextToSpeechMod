using System.IO;
using UnityEngine;

namespace SpeechMod.Unity.Utility;

public class AssetLoader
{
    public static Sprite LoadInternal(string folder, string file, Vector2Int size)
    {
        return CreateSpriteFromFile($"{Main.ModEntry.Path}Assets{Path.DirectorySeparatorChar}{folder}{Path.DirectorySeparatorChar}{file}", size);
    }

    public static Sprite CreateSpriteFromFile(string filePath, Vector2Int size, TextureFormat textureFormat = TextureFormat.DXT5)
    {
        var bytes = File.ReadAllBytes(filePath);
        var texture = new Texture2D(size.x, size.y, textureFormat, false);
        texture.mipMapBias = 15.0f;
        texture.LoadImage(bytes);
        return Sprite.Create(texture, new Rect(0, 0, size.x, size.y), new Vector2(0, 0));
    }
}