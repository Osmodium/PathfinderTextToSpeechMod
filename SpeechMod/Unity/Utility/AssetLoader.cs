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

    //private static Dictionary<string, GameObject> Objects = new();
    //public static Dictionary<string, Sprite> Sprites = new();
    //public static Dictionary<string, Mesh> Meshes = new();
    //public static Dictionary<string, Material> Materials = new();

    //public static void RemoveBundle(string loadAss, bool unloadAll = false)
    //{
    //    AssetBundle bundle;
    //    if (bundle = AssetBundle.GetAllLoadedAssetBundles().FirstOrDefault(x => x.name == loadAss))
    //        bundle.Unload(unloadAll);
    //    if (unloadAll)
    //    {
    //        Objects.Clear();
    //        Sprites.Clear();
    //        Meshes.Clear();
    //    }
    //}

    //public static UnityEngine.Object[] Assets;

    //public static void AddBundle(string loadAss)
    //{
    //    try
    //    {
    //        AssetBundle bundle;

    //        RemoveBundle(loadAss, true);

    //        var path = Path.Combine(Main.ModPath + loadAss);
    //        Main.Logger.Log($"loading from: {path}");

    //        bundle = AssetBundle.LoadFromFile(path);
    //        if (!bundle) 
    //            throw new Exception($"Failed to load AssetBundle! {Main.ModPath + loadAss}");

    //        Assets = bundle.LoadAllAssets();

    //        foreach (var obj in Assets)
    //        {
    //            Main.Logger.Log($"Found asset <{obj.name}> of type [{obj.GetType()}]");
    //        }

    //        foreach (var obj in Assets)
    //        {
    //            if (obj is GameObject gobj)
    //                Objects[obj.name] = gobj;
    //            else if (obj is Mesh mesh)
    //                Meshes[obj.name] = mesh;
    //            else if (obj is Sprite sprite)
    //                Sprites[obj.name] = sprite;
    //            else if (obj is Material material)
    //                Materials[obj.name] = material;
    //        }

    //        RemoveBundle(loadAss);
    //    }
    //    catch (Exception ex)
    //    {
    //        Main.Logger.Error("LOADING ASSET" + ex.Message + ex.StackTrace);
    //    }
    //}
}