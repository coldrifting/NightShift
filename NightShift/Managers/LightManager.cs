using System.Collections.Generic;
using NightShift.Data;
using NightShift.Utils;
using UnityEngine;
using Time = NightShift.Utils.Time;

namespace NightShift.Managers;

// Adjusts Lights
public static class LightManager
{
    private static Transform _shadowLight;
    
    private static readonly List<Material> SplashMaterials = [];
    private static readonly List<LightCache> Lights = [];

    private static readonly int TintColor = Shader.PropertyToID("_TintColor");
    private static readonly Color DefaultSplashColor = new(0.375f, 0.375f, 0.375f, 0.25f);
    
    public static void Init()
    {
        for (int i = 1; i <= 6; i++)
        {
            string path = $"SPH_interior_modern/SPH_Interior_Geometry/model_sph_interior_lights_v16/Component_744_1/rearWindowLightSplashes/Mesh{i}";
            GameObject splashFx = GameObject.Find(path);
            if (splashFx)
            {
                MeshRenderer mr = splashFx.GetComponent<MeshRenderer>();
                if (mr)
                {
                    var mat = mr.material;
                    if (mat)
                    {
                        SplashMaterials.Add(mat);
                    }
                }
            }
        }
        
        List<LightInfo> lightInfo =
        [
            new(EditorFacility.SPH, 3, 8f, "Lighting_Realtime/Realtime_ExteriorSun"),
            new(EditorFacility.SPH, 3, 0.3f, "Lighting_Realtime/Realtime_SpotlightWindow"),
            new(EditorFacility.SPH, 2, 8f, "Editors_DayLights/ExteriorSun"),
            new(EditorFacility.SPH, 1, 8f, "Editors_DayLights/ExteriorSun"),

            new(EditorFacility.VAB, 3, 5f, "VAB_interior_modern/Day Lights/SpotlightSun"),
            new(EditorFacility.VAB, 2, 5f, "Day Lights/SpotlightSun"),
            new(EditorFacility.VAB, 1, 5f, "Day Lights/SpotlightSun"),
        ];
        
        lightInfo.Initialize(Lights);

        if (Editor.Facility == EditorFacility.SPH)
        {
            _shadowLight = Editor.Level switch
            {
                >= 3 => GameObject.Find("SPHmodern/Lighting_Realtime/Realtime_Shadow Light")?.transform,
                2 => GameObject.Find("SPHlvl2/Editors_DayLights/Realtime_Shadow Light")?.transform,
                _ => GameObject.Find("SPHlvl1/Editors_DayLights/Realtime_Shadow Light")?.transform
            };
        }

        if (Editor.Facility == EditorFacility.VAB)
        {
            _shadowLight = Editor.Level switch
            {
                >= 3 => GameObject.Find("VABmodern/VAB_interior_modern/Day Lights/Shadow Light")?.transform,
                2 => GameObject.Find("VABlvl3/Day Lights/Shadow Light")?.transform,
                _ => GameObject.Find("VABlvl2/Day Lights/Shadow Light")?.transform
            };
        }
    }

    public static void Apply(double currentTime)
    {
        bool isNotNight = Time.GetTimeOfDay(currentTime) != TimeOfDay.Night;
        
        foreach (LightCache lightData in Lights)
        {
            lightData.Light.gameObject.SetActive(isNotNight);
            if (isNotNight)
            {
                lightData.Light.intensity = lightData.Intensity * AmbientLight.GetLightIntensityFactor(currentTime);
            }
        }

        foreach (Material splashMaterial in SplashMaterials)
        {
            splashMaterial.SetColor(TintColor, DefaultSplashColor * AmbientLight.GetLightIntensityFactor(currentTime));
        }

        if (_shadowLight)
        {
            double mirroredTime = Time.GetMirroredTime(currentTime);
            float val = (float)((mirroredTime - Time.Twilight2) / (Time.DayStart - Time.Twilight2));
            
            if (Editor.Facility == EditorFacility.SPH)
            {
                float rotation = Tools.Lerp(25, 90, val);
                _shadowLight.rotation = Editor.Level switch
                {
                    >= 3 => Quaternion.Euler(rotation, 0, 0),
                    2 => Quaternion.Euler(rotation, 180, 0),
                    _ => Quaternion.Euler(90, 0, 0)
                };
            }

            if (Editor.Facility == EditorFacility.VAB)
            {
                float rotation = Tools.Lerp(55, 90, val);
                _shadowLight.rotation = Editor.Level switch
                {
                    >= 3 => Quaternion.Euler(rotation, 270, 0),
                    2 => Quaternion.Euler(rotation, 270, 0),
                    _ => Quaternion.Euler(rotation, 270, 0)
                };
            }
        }
    }
}