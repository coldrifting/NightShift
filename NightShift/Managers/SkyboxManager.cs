using NightShift.Data;
using NightShift.Managers.Interface;
using NightShift.Utils;
using UnityEngine;

namespace NightShift.Managers;

// Replaces skybox shader and textures, and adjusts fog settings
internal class SkyboxManager : IDayNightManager
{
    private static readonly int FrontTex = Shader.PropertyToID("_FrontTex");
    private static readonly int BackTex = Shader.PropertyToID("_BackTex");
    private static readonly int LeftTex = Shader.PropertyToID("_LeftTex");
    private static readonly int RightTex = Shader.PropertyToID("_RightTex");
    private static readonly int UpTex = Shader.PropertyToID("_UpTex");
    private static readonly int DownTex = Shader.PropertyToID("_DownTex");
    
    private static readonly int Rotation = Shader.PropertyToID("_Rotation");
    private static readonly int TimeOffset = Shader.PropertyToID("_TimeOffset");
    private static readonly int SunSize = Shader.PropertyToID("_SunSize");

    public void Init()
    {
        // Galaxy Cube Order: -X, +X, -Y, +Y, -Z, +Z
        Transform galaxy = GalaxyCubeControl.Instance.gameObject.transform;
        
        Texture[] galTex = new Texture[6];
        for (int i = 0; i < 6; i++)
        {
            var child = galaxy.GetChild(i);
            if (child)
            {
                var mr = child.GetComponent<MeshRenderer>();
                if (mr)
                {
                    galTex[i] = mr.material.mainTexture;
                    continue;
                }
            }

            galTex[i] = Texture2D.blackTexture;
        }

        Shader skyboxShader = Shabby.Shabby.FindShader("NightShift/SkyboxDynamic");
        if (!skyboxShader)
        {
            NightShift.Warn("Unable to find skybox shader named: \"NightShift/SkyboxDynamic\". Is the shader bundle loaded?");
        }
        
        Material skyboxMat = new Material(skyboxShader)
        {
            name = "Skybox (Night)",
            mainTexture = new Cubemap(galTex[0].width, TextureFormat.RGB24, true),
            color = Color.black
        };
        
        skyboxMat.SetTexture(RightTex, galTex[0]);
        skyboxMat.SetTexture(LeftTex, galTex[1]);
        skyboxMat.SetTexture(DownTex, galTex[2]);
        skyboxMat.SetTexture(UpTex, galTex[3]);
        skyboxMat.SetTexture(BackTex, galTex[4]);
        skyboxMat.SetTexture(FrontTex, galTex[5]);

        RenderSettings.skybox = skyboxMat;
    }

    public void Apply(double currentTime)
    {
        // Adjust horizon fog
        RenderSettings.fogColor = AmbientLight.GetFogColor(currentTime);
        
        int rotation = Editor.Facility == EditorFacility.SPH ? -60 : 0;
        RenderSettings.skybox.SetFloat(Rotation, rotation);
        RenderSettings.skybox.SetFloat(TimeOffset, (float)((currentTime - 0.25f + 1.0f) * 360.0 % 360.0));
        RenderSettings.skybox.SetFloat(SunSize, 2.0f);
    }
}