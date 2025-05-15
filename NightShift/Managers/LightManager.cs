using System.Collections.Generic;
using UnityEngine;

namespace NightShift.Managers;

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

    public void Apply(TimeOfDay timeOfDay)
    {
        bool isDay = timeOfDay == TimeOfDay.Day;
        
        // Fog Color
        RenderSettings.fogColor = timeOfDay switch
        {
            TimeOfDay.Day => new(0.5647f, 0.6784f, 0.7176f, 1),
            TimeOfDay.Twilight => new(0.32f, 0.2f, 0.16f, 1),
            TimeOfDay.Night => Color.black,
            _ => Color.black
        };
        
        // Fog Start Dist
        RenderSettings.fogStartDistance = timeOfDay == TimeOfDay.Day ? 400 : 200;
        
        // Enable Ambient light on exterior meshes
        foreach (MeshRenderer mr in _objectAmbientOcclusion)
        {
            if (!mr)
                continue;
            
            // Ground is special
            if (mr.name == "ksc_terrain")
            {
                mr.realtimeLightmapIndex = !isDay ? 0 : -1;
            }
            else
            {
                mr.lightmapIndex = !isDay ? 0 : -1;
            }
        }
    }
}