using System.Collections.Generic;
using NightShift.Data;
using NightShift.Utils;
using UnityEngine;

namespace NightShift.Managers;

// Overlays night textures on windows when relevant through use of a custom shader
public static class WindowManager
{
    private static readonly int OverlayTex = Shader.PropertyToID("_OverlayTex");
    // ReSharper disable once StringLiteralTypo
    private static readonly int EmissiveTex = Shader.PropertyToID("_Illum");
    private static readonly int SkyMaskTex = Shader.PropertyToID("_SkyMaskTex");
    private static readonly int SkyColor = Shader.PropertyToID("_SkyColor");

    // mesh, id, textureProperty, materialIndex
    private static List<Material> _materials;
    
    public static void Init()
    {
        List<MeshTextureInfo> meshTextureInfo =
        [
            // SPH
            new(EditorFacility.SPH, 3, 1, Constants.SphLvl3InteriorMain, hasLightmap: true, hasOverlayTex: true, hasEmissiveTex: true, hasSkyMaskTex: true, overrideColor: new Color(0.5f, 0.5f, 0.5f)),
            new(EditorFacility.SPH, 3, 2, Constants.SphLvl3InteriorMain, hasLightmap: true, hasOverlayTex: true, hasEmissiveTex: true, hasSkyMaskTex: true),
            
            new(EditorFacility.SPH, 3, 4, Constants.SphLvl3InteriorMain, hasLightmap: true),
            new(EditorFacility.SPH, 3, 5, Constants.SphLvl3InteriorMain, hasLightmap: true),
            new(EditorFacility.SPH, 3, 6, Constants.SphLvl3InteriorMain, hasLightmap: true),
            new(EditorFacility.SPH, 3, 7, Constants.SphLvl3InteriorMain, hasLightmap: true),
            new(EditorFacility.SPH, 3, 8, Constants.SphLvl3InteriorMain, hasLightmap: true),
            
            new(EditorFacility.SPH, 3, 0, Constants.SphLvl3SideGates, hasOverlayTex: true,  hasEmissiveTex: true),
            
            new(EditorFacility.SPH, 2, 0, Constants.SphLvl2Windows, hasOverlayTex: true),
            new(EditorFacility.SPH, 2, 1, Constants.SphLvl2Windows, hasOverlayTex: true, overrideLevel: 1),
            new(EditorFacility.SPH, 2, 2, Constants.SphLvl2Door1,   hasOverlayTex: true),
            new(EditorFacility.SPH, 2, 2, Constants.SphLvl2Door2,   hasOverlayTex: true),
            new(EditorFacility.SPH, 2, 2, Constants.SphLvl2Door3,   hasOverlayTex: true),
            new(EditorFacility.SPH, 2, 2, Constants.SphLvl2Door4,   hasOverlayTex: true),
            
            new(EditorFacility.SPH, 1, 1, Constants.SphLvl1Window,  hasOverlayTex: true),

            // VAB
            new(EditorFacility.VAB, 3, 0, Constants.VabLvl3Windows, hasLightmap: true, hasOverlayTex: true, hasEmissiveTex: true),
            new(EditorFacility.VAB, 3, 1, Constants.VabLvl3Windows, hasLightmap: true, hasOverlayTex: true, hasEmissiveTex: true),
            new(EditorFacility.VAB, 3, 0, Constants.VabLvl3Walls,   hasLightmap: true, hasOverlayTex: true, hasEmissiveTex: true),
            
            new(EditorFacility.VAB, 1, 1, Constants.VabLvl1Windows, hasOverlayTex: true)
        ];

        _materials = [];
        foreach (MeshTextureInfo mti in meshTextureInfo)
        {
            if (mti.Facility == Editor.Facility && mti.Level == Editor.Level)
            {
                MeshRenderer mr = mti.MeshRenderer;
                if (mr)
                {
                    var mat = mr.materials[mti.MaterialIndex];
                    if (mat)
                    {
                        mat.shader = ShaderInfo.ShaderNightShiftOverlaysEmissiveLightmapper;
                        if (mti.HasLightmap)
                        {
                            mat.EnableKeyword("_LIGHTMAP_ON");
                        }

                        if (mti.HasOverlayTex)
                        {
                            mat.SetTexture(OverlayTex, mti.OverlayTex);
                        }
                        
                        if (mti.HasEmissiveTex)
                        {
                            mat.SetTexture(EmissiveTex, mti.EmissiveTex);
                        }

                        if (mti.HasSkyMaskTex)
                        {
                            mat.SetTexture(SkyMaskTex, mti.SkyMaskTex);
                        }

                        if (mti.OverrideColor != null)
                        {
                            mat.color = mti.OverrideColor.Value;
                        }

                        _materials.Add(mat);
                    }
                }
            }
        }
    }

    public static void Apply(double currentTime)
    {
        foreach (Material mat in _materials)
        {
            if (mat)
            {
                mat.SetColor(SkyColor, AmbientLight.GetSkyColor(currentTime));
            }
        }
    }
}