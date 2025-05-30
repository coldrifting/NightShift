using System.Collections.Generic;
using NightShift.Utils;
using UnityEngine;

namespace NightShift.Data;

public class LightInfo(EditorFacility facility, int level, float intensity, string path)
{
    public readonly EditorFacility Facility = facility;
    public readonly int Level = level;
    
    public readonly float Intensity = intensity;

    private GameObject GameObject => Editor.RootInteriorObject?.transform.Find(path)?.gameObject;
    public Light Light => GameObject ? GameObject?.GetComponent<Light>() : null;
}

public class LightCache(Light light, float intensity)
{
    public readonly Light Light = light;
    public readonly float Intensity = intensity;
}

public static class LightInfoExt
{
    public static void Initialize(this List<LightInfo> allLightInfo, List<LightCache> lights) 
    {
        lights.Clear();
        lights.Capacity = allLightInfo.Count;
        foreach (var lightInfo in allLightInfo)
        {
            Light light = lightInfo.Light;
            if (light)
            {
                lights.Add(new(light, lightInfo.Intensity));
            }
        }
    }
}