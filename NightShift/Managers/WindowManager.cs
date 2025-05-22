using System.Collections.Generic;
using NightShift.Data;
using NightShift.Managers.Interface;
using NightShift.Utils;
using UnityEngine;

namespace NightShift.Managers;

// Overlays night textures on windows when relevant through use of a custom shader
public class WindowManager : IDayNightManager
{
    private static readonly int NightOverlayTex = Shader.PropertyToID("_NightOverlayTex");
    private static readonly int NightCrossFade = Shader.PropertyToID("_NightCrossFade");
    private static readonly int UseLightmap = Shader.PropertyToID("_UseLightmap");

    // mesh, id, textureProperty, materialIndex
    private List<Material> _materials;
    
    public void Init()
    {
        List<MeshTextureInfo> meshTextureInfo =
        [
            new(EditorFacility.SPH, 3, Constants.SphLvl3InteriorMain, 1),
            new(EditorFacility.SPH, 3, Constants.SphLvl3InteriorMain, 2),
            new(EditorFacility.SPH, 3, Constants.SphLvl3SideGates, 0),
            new(EditorFacility.SPH, 2, Constants.SphLvl2Windows, 0),
            new(EditorFacility.SPH, 2, Constants.SphLvl2Windows, 1),
            new(EditorFacility.SPH, 2, Constants.SphLvl2Door1, 2),
            new(EditorFacility.SPH, 2, Constants.SphLvl2Door2, 2),
            new(EditorFacility.SPH, 2, Constants.SphLvl2Door3, 2),
            new(EditorFacility.SPH, 2, Constants.SphLvl2Door4, 2),
            new(EditorFacility.SPH, 1, Constants.SphLvl1Window, 1),

            new(EditorFacility.VAB, 3, Constants.VabLvl3Windows, 0),
            new(EditorFacility.VAB, 3, Constants.VabLvl3Windows, 1),
            new(EditorFacility.VAB, 3, Constants.VabLvl3Walls, 0),
            new(EditorFacility.VAB, 1, Constants.VabLvl1Windows, 1)
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
                        mat.shader = ShaderInfo.ShaderNightShiftEmissiveLightmapper;
                        mat.SetTexture(NightOverlayTex, mti.Texture);
                        if (mat.name.StartsWith(Constants.SphLvl3SideGates))
                        {
                            mat.SetFloat(UseLightmap, 0);
                        }
                        _materials.Add(mat);
                    }
                }
            }
        }
    }

    public void Apply(double currentTime)
    {
        foreach (Material mat in _materials)
        {
            if (mat)
            {
                mat.SetFloat(NightCrossFade, 1 - AmbientLight.GetLightIntensityFactor(currentTime));
            }
        }
    }
}