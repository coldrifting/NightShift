using System.Collections.Generic;
using NightShift.Data;
using NightShift.Managers.Interface;
using UnityEngine;

namespace NightShift.Managers;

// Adjusts Lights
public class LightManager : IDayNightManager
{
    private readonly List<Material> _splashMaterials = [];
    private readonly List<LightCache> _lights = [];

    private static readonly int TintColor = Shader.PropertyToID("_TintColor");
    private readonly Color _defaultSplashColor = new(0.375f, 0.375f, 0.375f, 0.25f);
    
    public void Init()
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
                        _splashMaterials.Add(mat);
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
        
        lightInfo.Initialize(_lights);
    }

    public void Apply(double currentTime)
    {
        bool isNotNight = Time.GetTimeOfDay(currentTime) != TimeOfDay.Night;
        
        foreach (LightCache lightData in _lights)
        {
            lightData.Light.gameObject.SetActive(isNotNight);
            if (isNotNight)
            {
                lightData.Light.intensity = lightData.Intensity * AmbientLight.GetLightIntensityFactor(currentTime);
            }
        }

        foreach (Material splashMaterial in _splashMaterials)
        {
            splashMaterial.SetColor(TintColor, _defaultSplashColor * AmbientLight.GetLightIntensityFactor(currentTime));
        }
    }
}