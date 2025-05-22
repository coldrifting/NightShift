using System.Collections.Generic;
using NightShift.Data;
using NightShift.Managers.Interface;
using NightShift.Utils;
using UnityEngine;

namespace NightShift.Managers;

// Replaces shaders for outside objects for fine-tuned control of ambient light colors
public class AmbientManager : IDayNightManager
{
    private static readonly int AmbientLightOverride = Shader.PropertyToID("_AmbientLightOverride");
    
    private static readonly List<(string, ShaderInfo)> MaterialInfo = 
    [
        ( "ksc_exterior_terrain_asphalt_diffuse",  ShaderInfo.ShaderMapDiffuse ),
        ( "ksc_exterior_terrain_tile",             ShaderInfo.ShaderMapDiffuse ),
        ( "ksp_pad_concrete_mat",                  ShaderInfo.ShaderMapDiffuse ),
        ( "Pole",                                  ShaderInfo.ShaderMapSpecular ),
        ( "ksp_pad_sphereTank_mat",                ShaderInfo.ShaderMapSpecular ),
        ( "model_tracking_exterior",               ShaderInfo.ShaderMapSpecular ),
        ( "model_vab_exterior_lines",              ShaderInfo.ShaderMapSpecular ),
        ( "model_vab_exterior_props",              ShaderInfo.ShaderMapSpecular ),
        ( "model_vab_exterior_tile",               ShaderInfo.ShaderMapSpecular ),
        ( "model_vab_exterior_details",            ShaderInfo.ShaderMapEmissiveSpecular ),
        ( "kap_pad_launchPad_mat",                 ShaderInfo.ShaderMapBumpedSpecular ),
        ( "ksp_pad_cylTank_mat",                   ShaderInfo.ShaderMapBumpedSpecular ),
        ( "ksp_pad_pipes_mat",                     ShaderInfo.ShaderMapBumpedSpecular ),
        ( "ksp_pad_waterTower_mat",                ShaderInfo.ShaderMapBumpedSpecular ),
        ( "podMemorialBase",                       ShaderInfo.ShaderMapBumpedSpecular ),
        ( "podMemorialPod",                        ShaderInfo.ShaderMapBumpedSpecular ),
        ( "ksc_exterior_terrain_asphalt_line",     ShaderInfo.ShaderMapGroundKsc ),
        ( "ksc_exterior_terrain_grass",            ShaderInfo.ShaderMapGroundKsc ),
        ( "ksc_exterior_terrain_grass_VABLvl03",   ShaderInfo.ShaderMapGroundKsc ),
        ( "ksc_exterior_terrain_grass_crawlerway", ShaderInfo.ShaderMapGroundKsc ),
        ( "ksc_exterior_terrain_grass_runway",     ShaderInfo.ShaderMapGroundKsc ),
        ( "ksc_terrain",                           ShaderInfo.ShaderMapLegacyDiffuse ),
        ( "crater",                                ShaderInfo.ShaderMapLegacyDiffuse )
    ];

    private static readonly List<string> MaterialModelBlockList = ["model_sph_interior_main_v16"];

    private readonly List<Material> _materials = [];
    
    public void Init()
    {
        IndexMaterials();
    }

    public void Apply(double currentTime)
    {
        Color nightAmbientLight = AmbientLight.GetAmbientColor(currentTime);
        foreach (var material in _materials)
        {
            if (material)
            {
                material.SetColor(AmbientLightOverride, nightAmbientLight);
            }
        }
    }

    private void IndexMaterials(bool cache = true)
    {
        _materials.Clear();

        List<MeshRenderer> meshes = [];
        if (cache)
        {
            meshes.AddRange(Editor.RootExteriorObject?.transform.GetAllChildComponents<MeshRenderer>() ?? []);
            meshes.AddRange(Editor.RootInteriorObject?.transform.GetAllChildComponents<MeshRenderer>() ?? []);
        }
        else
        {
            meshes.AddRange(GameObject.Find("localSpace/Kerbin/Kerbin/KSC/SpaceCenter")?.transform.GetAllChildComponents<MeshRenderer>() ?? []);
        }
        foreach (MeshRenderer mr in meshes)
        {
            if (!mr)
            {
                continue;
            }

            if (MaterialModelBlockList.Contains(mr.name))
            {
                continue;
            }
            
            foreach (Material mat in mr.materials)
            {
                if (!mat)
                {
                    continue;
                }
                
                var parent = mr.gameObject.transform.parent;
                if (parent)
                {
                    if (parent.name == "model_prop_truck_h01") {
                        int originalRenderQueue = mat.renderQueue;
                        mat.shader = ShaderInfo.ShaderMapDiffuse.NightShader;
                        _materials.Add(mat);
                        mr.gameObject.layer = 15; // Move back to Scenery Layer (Deferred Changes this)
                        mat.renderQueue = originalRenderQueue;
                    }
                    else if (parent.name.StartsWith("model_prop_truck")) {
                        continue;
                    }
                }
                
                foreach ((string, ShaderInfo) kv in MaterialInfo)
                {
                    if (mat.name.StartsWith(kv.Item1))
                    {
                        int originalRenderQueue = mat.renderQueue;
                        mat.shader = cache ? kv.Item2.NightShader : kv.Item2.DayShader;
                        mat.renderQueue = originalRenderQueue;
                        if (cache)
                        {
                            _materials.Add(mat);
                        }
                    }
                }
            }
        }
    }
    
    // Since Editor shader changes persist across loads, revert those changes on Flight and SpaceCenter Views
    public void Reset()
    {
        IndexMaterials(false);
    }
}