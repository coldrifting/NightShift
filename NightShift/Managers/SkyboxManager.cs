using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NightShift.Managers;

// Most of this skybox code comes from the LightsOut mod.
// Big thanks to that mod's authors for their helpful source code.

// "Replaces" skybox by using a new camera synced to editor camera,
// so we can edit the rotation of the skybox
internal class SkyboxManager : IDayNightManager
{
    private Material _daySky;
    private Material _twilightSky;
    private Material _nightSky;

    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private static readonly int UpTex = Shader.PropertyToID("_UpTex");
    private static readonly int BackTex = Shader.PropertyToID("_BackTex");
    private static readonly int LeftTex = Shader.PropertyToID("_LeftTex");
    private static readonly int FrontTex = Shader.PropertyToID("_FrontTex");
    private static readonly int RightTex = Shader.PropertyToID("_RightTex");
    private static readonly int DownTex = Shader.PropertyToID("_DownTex");


    public SkyboxManager()
    {
        Init();
    }

    private static Material BuildNightSkybox()
    {
    
        Texture2D[] cube = new Texture2D[6];
        
        MeshRenderer[] renders = GalaxyCubeControl.Instance.transform.GetComponentsInChildren<MeshRenderer>();
        foreach (var render in renders)
        {
            switch (render.name)
            {
                case "YP":
                    cube[0] = render.material.GetTexture(MainTex) as Texture2D;
                    break;
                case "ZN":
                    cube[1] = render.material.GetTexture(MainTex) as Texture2D;
                    break;
                case "XP":
                    cube[2] = render.material.GetTexture(MainTex) as Texture2D;
                    break;
                case "ZP":
                    cube[3] = render.material.GetTexture(MainTex) as Texture2D;
                    break;
                case "XN":
                    cube[4] = render.material.GetTexture(MainTex) as Texture2D;
                    break;
                case "YN":
                    cube[5] = render.material.GetTexture(MainTex) as Texture2D;
                    break;
            }
        }
            
        Material output = new Material(RenderSettings.skybox.shader)
        {
            name = "Skybox (Night)",
            mainTexture = new Cubemap(cube[0].width, TextureFormat.RGB24, true),
            color = Color.white
        };
        
        // Realign a
        output.SetTexture(UpTex, cube[0]);
        output.SetTexture(BackTex, cube[3]);
        output.SetTexture(LeftTex, cube[4]);
        output.SetTexture(FrontTex, cube[1]);
        output.SetTexture(RightTex, cube[2]);
        output.SetTexture(DownTex, cube[5]);

        RenderSettings.skybox = output;
        
        return output;
    }
    
    private static Material BuildSkybox(string cubeMapName)
    {
        var top = GameDatabase.Instance.GetTexture($"NightShift/Cubemaps/{cubeMapName}/top", false);
        var side = GameDatabase.Instance.GetTexture($"NightShift/Cubemaps/{cubeMapName}/side", false);
        var bottom = GameDatabase.Instance.GetTexture($"NightShift/Cubemaps/{cubeMapName}/bottom", false);
        
        Material output = new Material(RenderSettings.skybox.shader)
        {
            name = $"Skybox ({cubeMapName})",
            mainTexture = new Cubemap(side.width, TextureFormat.RGB24, false),
            color = Color.white
        };
        
        output.SetTexture(UpTex, top);
        output.SetTexture(BackTex, side);
        output.SetTexture(LeftTex, side);
        output.SetTexture(FrontTex, side);
        output.SetTexture(RightTex, side);
        output.SetTexture(DownTex, bottom);
        
        return output;
    }

    private static Material GetDefaultSkybox()
    {
        if (RenderSettings.skybox.name != "Sunny3 Skybox")
        {
            Material[] materials = Object.FindObjectsOfType<Material>();
            foreach (var material in materials)
            {
                if (material.name == "Sunny3 Skybox")
                {
                    return material;
                }
            }
        }
        
        return new(RenderSettings.skybox);
    }
    
    public void Init()
    {
        try
        {
            _daySky = GetDefaultSkybox();
            
            _twilightSky = BuildSkybox("Twilight");
            
            _nightSky = BuildNightSkybox();
            
            RenderSettings.skybox = _daySky;
        }
        catch (Exception e)
        {
            Debug.Log(e.StackTrace);
            throw;
        }
    }

    public void Apply(TimeOfDay timeOfDay)
    {
        RenderSettings.skybox = timeOfDay switch
        {
            TimeOfDay.Twilight => _twilightSky,
            TimeOfDay.Night => _nightSky,
            _ => _daySky
        };
    }
}