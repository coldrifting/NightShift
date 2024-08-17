using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Unity.Collections;
using UnityEngine;

namespace NightShift;

public static class TextureCache
{
    private static readonly Dictionary<string, (Texture2D dayTex, Texture2D nightTex)> Cache = new();


    public static Texture2D GetTexture(string id, bool isDayTexture)
    {
        if (Cache.TryGetValue(id, out var textures))
        {
            return isDayTexture ? textures.dayTex : textures.nightTex;
        }
        
        return null;
    }
    
    public static void BuildCache(List<(MeshRenderer, string, int, int)> textureList)
    {
        foreach ((MeshRenderer mr, string id, int texId, int matIndex) in textureList)
        {
            if (Cache.ContainsKey(id))
                continue;
            
            if (matIndex >= mr.materials.Length)
                continue;
            
            Texture2D dayTex = mr.materials[matIndex].GetTexture(texId) as Texture2D;
            if (!dayTex)
                continue;
            
            Texture2D nightTex = CacheDayNightTex(dayTex, id).nightTex;
            if (!nightTex)
                continue;
                        
            Cache[id] = (dayTex, nightTex);
        }
    }

    private static string ModPath => Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
    private static (Texture2D dayTex, Texture2D nightTex) CacheDayNightTex(Texture2D dayTex, string id)
    {
        if (id.EndsWith("_Illum"))
        {
            return (dayTex, Texture2D.blackTexture);
        }

        Texture2D nightTexCached = GameDatabase.Instance.GetTexture($"NightShift/Assets/Cache/{id}", false);
        if (nightTexCached)
        {
            return (dayTex, nightTexCached);
        }
        
        Texture2D nightTexOverlay = GameDatabase.Instance.GetTexture($"NightShift/Assets/{id}", false);
        if (!nightTexOverlay) 
            return (null, null);
        
        Texture2D nightTex = Merge(dayTex, nightTexOverlay);
        
        // Cache output for next KSP run
        if (!File.Exists(ModPath + $"/NightShift/Assets/Cache/{id}.png"))
        {
            Directory.CreateDirectory(ModPath + $"/NightShift/Assets/Cache");
            File.WriteAllBytes(ModPath + $"/NightShift/Assets/Cache/{id}.png", nightTex.EncodeToPNG());
        }

        return (dayTex, nightTex);
    }
    
    private static Texture2D Merge(Texture2D baseTex, Texture2D overlayTex)
    {
        NativeArray<Color32> baseTexData = DuplicateTexture(baseTex).GetRawTextureData<Color32>();
        NativeArray<Color32> overlayTexData =  DuplicateTexture(overlayTex).GetRawTextureData<Color32>();
        
        Texture2D output = new Texture2D(baseTex.width, baseTex.height, TextureFormat.RGBA32, true);
        NativeArray<Color32> outputData = output.GetRawTextureData<Color32>();
        for (int i = 0; i < baseTex.width * baseTex.height; i++)
        {
            if (overlayTexData[i].a > 0)
            {
                // Preserve base texture alpha
                outputData[i] = new(overlayTexData[i].r, overlayTexData[i].g, overlayTexData[i].b, baseTexData[i].a);
            }
            else
            {
                outputData[i] = baseTexData[i];
            }
        }
        output.Apply(true);

        return output;
    }
    
    private static Texture2D DuplicateTexture(Texture source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
}