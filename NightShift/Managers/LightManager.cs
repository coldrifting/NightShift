using System.Collections.Generic;
using UnityEngine;

namespace NightShift;

// Adjusts Ambient fog and light properties
public class LightManager : IDayNightManager
{
    private readonly List<MeshRenderer> _objectAmbientOcclusion = [];
    
    public LightManager()
    {
        Init();
    }
    
    public void Init()
    {
        _objectAmbientOcclusion.Clear();
        
        Transform sceneryRoot = Tools.GetRootExteriorTransform();
        Transform interiorRoot = Tools.GetRootInteriorTransform();
        
        // Find objects that will need to have their ambient occlusion removed
        Queue<Transform> l = new();
        if (!sceneryRoot)
        {
            return;
        }
        
        l.Enqueue(sceneryRoot);
        while (l.Count > 0)
        {
            Transform cur = l.Dequeue();

            if (cur.gameObject.GetComponent<MeshRenderer>() is { } mr)
            {
                _objectAmbientOcclusion.Add(mr);
            }
            
            foreach (Transform t in cur)
            {
                l.Enqueue(t);
            }
        }

        switch (Tools.Level)
        {
            case 3:
            _objectAmbientOcclusion.TryAddFromPath(interiorRoot,
                Tools.IsSph
                    ? "SPH_interior_modern/model_sph_exterior_ground_v46n"
                    : "VAB_interior_modern/model_vab_exterior_ground_v46n");
                break;
            
            case 2:
            _objectAmbientOcclusion.TryAddFromPath(interiorRoot,
                Tools.IsSph
                    ? "SPH_lev2_groundPlane"
                    : "VAB_lev2_groundPlane");
                break;
            
            case 1:
            _objectAmbientOcclusion.TryAddFromPath(interiorRoot,
                Tools.IsSph
                    ? "SPH_lev1_ground"
                    : "VAB_lev1_interior/VAB_lev1_groundPlane");
                break;
        }
    }

    public void Switch(bool isDay)
    {
        // Fog Color
        RenderSettings.fogColor = isDay ? new(0.5647f, 0.6784f, 0.7176f, 1) : Color.black;
        
        // Enable Ambient light on exterior meshes
        foreach (MeshRenderer mr in _objectAmbientOcclusion)
        {
            if (!mr)
                continue;
            
            // Ground is special
            if (mr.name == "ksc_terrain")
            {
                mr.realtimeLightmapIndex = isDay ? -1 : 0;
            }
            else
            {
                mr.lightmapIndex = isDay ? -1 : 0;
            }
        }
    }
}