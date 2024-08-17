using System.Collections.Generic;
using UnityEngine;

namespace NightShift;

// Adjusts textures of objects
public class TextureManager : IDayNightManager
{
    private const string Emit = "Blank";
    
    private const string SphLevel1 = "SPH_1";
    private const string SphLevel2 = "SPH_2";
    private const string SphLevel3 = "SPH_3";

    private const string VabLevel1 = "VAB_1";
    private const string VabLevel3 = "VAB_3";
    
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private static readonly int Illum = Shader.PropertyToID("_Illum");
    
    private readonly List<(MeshRenderer, string, int, int)> _meshLookup = [];

    public TextureManager()
    {
        Init();
    }
    
    public void Init()
    {
        _meshLookup.Clear();
        
        Transform interiorRoot = Tools.GetRootInteriorTransform();
        
        // Find objects to patch
        if (Tools.IsSph)
        {
            switch (Tools.Level)
            {
                case 3:
                    // SPH - Level 3
                    if (interiorRoot.TryGetCompFromPath<MeshRenderer>(
                            "SPH_interior_modern/SPH_Interior_Geometry/model_sph_interior_main_v16") is { } mr31)
                    {
                        _meshLookup.Add((mr31, SphLevel3, MainTex, 1));
                        _meshLookup.Add((mr31, SphLevel3, MainTex, 2));
                        _meshLookup.Add((mr31, SphLevel3 + "_Illum", Illum, 1));
                        _meshLookup.Add((mr31, SphLevel3 + "_Illum", Illum, 2));
                    }
                    
                    if (interiorRoot.TryGetCompFromPath<MeshRenderer>(
                            "SPH_interior_modern/SPH_Interior_Geometry/model_sph_interior_gates_v16/Component_755_1/Mesh1") is { } mr32)
                    {
                        _meshLookup.Add((mr32, SphLevel3, MainTex, 0));
                        _meshLookup.Add((mr32, SphLevel3 + "_Illum", Illum, 0));
                    }
                    break;
                
                case 2:
                    // SPH - Level 2
                    if (interiorRoot.TryGetCompFromPath<MeshRenderer>(
                            "SPH_2_interior/SPH_2_windows") is { } mr21)
                    {
                        _meshLookup.Add((mr21, SphLevel2, MainTex, 0));
                        _meshLookup.Add((mr21, SphLevel2 + "_Illum", Illum, 0));
                        _meshLookup.Add((mr21, SphLevel1, MainTex, 1));
                        _meshLookup.Add((mr21, SphLevel1 + "_Illum", Illum, 1));
                    }
                    
                    if (interiorRoot.TryGetCompFromPath<MeshRenderer>(
                            "SPH_2_interior/SPH_2_door1") is { } mr22)
                    {
                        _meshLookup.Add((mr22, SphLevel2, MainTex, 2));
                        _meshLookup.Add((mr22, SphLevel2 + "_Illum", Illum, 2));
                    }
                    
                    if (interiorRoot.TryGetCompFromPath<MeshRenderer>(
                            "SPH_2_interior/SPH_2_door2") is { } mr23)
                    {
                        _meshLookup.Add((mr23, SphLevel2, MainTex, 2));
                        _meshLookup.Add((mr23, SphLevel2 + "_Illum", Illum, 2));
                    }
                    
                    if (interiorRoot.TryGetCompFromPath<MeshRenderer>(
                            "SPH_2_interior/SPH_2_door3") is { } mr24)
                    {
                        _meshLookup.Add((mr24, SphLevel2, MainTex, 2));
                        _meshLookup.Add((mr24, SphLevel2 + "_Illum", Illum, 2));
                    }
                    
                    if (interiorRoot.TryGetCompFromPath<MeshRenderer>(
                            "SPH_2_interior/SPH_2_door4") is { } mr25)
                    {
                        _meshLookup.Add((mr25, SphLevel2, MainTex, 2));
                        _meshLookup.Add((mr25, SphLevel2 + "_Illum", Illum, 2));
                    }
                    
                    break;
                
                case 1:
                    // SPH - Level 1
                    if (interiorRoot.TryGetCompFromPath<MeshRenderer>(
                            "SPH-1_Interior/group37_polySurface813") is { } mr11)
                    {
                        _meshLookup.Add((mr11, SphLevel1, MainTex, 1));
                        _meshLookup.Add((mr11, SphLevel1 + "_Illum", Illum, 1));
                    }
                    break;
            }
        }
        else
        {
            switch (Tools.Level)
            {
                case 3:
                    // VAB - Level 3
                    if (interiorRoot.TryGetCompFromPath<MeshRenderer>(
                            "VAB_interior_modern/VAB_Interior_Geometry/model_vab_windows") is { } mr31)
                    {
                        _meshLookup.Add((mr31, VabLevel3, MainTex, 0));
                        _meshLookup.Add((mr31, VabLevel3, MainTex, 1));
                        _meshLookup.Add((mr31, VabLevel3 + "_Illum", Illum, 0));
                        _meshLookup.Add((mr31, VabLevel3 + "_Illum", Illum, 1));
                    };
                    
                    if (interiorRoot.TryGetCompFromPath<MeshRenderer>(
                            "VAB_interior_modern/VAB_Interior_Geometry/model_vab_interior_walls_v17") is { } mr32)
                    {
                        _meshLookup.Add((mr32, VabLevel3, MainTex, 0));
                        _meshLookup.Add((mr32, VabLevel3 + "_Illum", Illum, 0));
                    }

                    break;
                case 2:
                    // VAB - Level 2
                    // No windows to patch :)
                    break;
                case 1:
                    // VAB - Level 1
                    if (interiorRoot.TryGetCompFromPath<MeshRenderer>(
                            "VAB_lev1_interior/INTERIOR_VAB_lev2 1/INTERIOR_VAB_lev2 1_MeshPart1") is { } mr11)
                    {
                        _meshLookup.Add((mr11, VabLevel1, MainTex, 1));
                        _meshLookup.Add((mr11, VabLevel1 + "_Illum", Illum, 1));
                    }
                    break;
            }
        }

        TextureCache.BuildCache(_meshLookup);
    }

    public void Switch(bool isDay)
    {
        foreach ((MeshRenderer mr, string id, int texType, int matIndex) in _meshLookup)
        {
            if (matIndex >= mr.materials.Length)
            {
                continue;
            }

            Texture2D tex = TextureCache.GetTexture(id, isDay);
            if (tex)
                mr.materials[matIndex].SetTexture(texType, tex);
        }
    }
}