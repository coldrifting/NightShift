using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace NightShift
{
    internal class LightManager
    {
        private readonly AmbientMode OriginalAmbientMode;
        private readonly Color OriginalAmbientLightColor;
        private readonly float OriginalAmbientLightIntensity;

        private readonly GameObject rootGameObject;

        private readonly String[] dayLights = new string[]
        {
            "Realtime_ExteriorSun",
            "Realtime_SpotlightScenery",
            "Realtime_SpotlightWindow",
            "Scaledspace SunLight",
            "SpotlightSun",
            "SunLight",
        };

        internal LightManager()
        {
            rootGameObject = GameObject.Find("NightShift");
            if (rootGameObject == null)
            {
                Debug.LogError("[NightShift] Unable to find parent gameobject for lights. This might cause issues.");
            }

            // Store default Ambient light settings for daytime
            OriginalAmbientMode = RenderSettings.ambientMode;
            OriginalAmbientLightIntensity = RenderSettings.ambientIntensity;
            OriginalAmbientLightColor = RenderSettings.ambientLight;

            GenerateNewNightLights();
        }

        internal void EnableDayMode(bool isDayMode)
        {
            // Ambient Light settings
            if (isDayMode)
            {
                RenderSettings.ambientMode = OriginalAmbientMode;
                RenderSettings.ambientLight = OriginalAmbientLightColor;
                RenderSettings.ambientIntensity = OriginalAmbientLightIntensity;
                RenderSettings.fog = true;
            }
            else
            {
                RenderSettings.ambientMode = AmbientMode.Skybox;
                RenderSettings.ambientLight = new Color(0.05f, 0.05f, 0.05f);
                RenderSettings.ambientIntensity = 0.05f;
                RenderSettings.fog = false;
            }

            // Default Exterior Lighting
            Light[] dayLightsVAB = new Light[0];
            Light[] dayLightsSPH = new Light[0];

            GameObject dayLightsVABRoot = GameObject.Find("Day Lights");
            if (dayLightsVABRoot != null)
                dayLightsVAB = dayLightsVABRoot.GetComponentsInChildren<Light>();

            GameObject dayLightsSPHRoot = GameObject.Find("Lighting_Realtime");
            if (dayLightsSPHRoot != null)
                dayLightsSPH = dayLightsSPHRoot.GetComponentsInChildren<Light>();

            foreach (Light l in dayLightsVAB)
            {
                if (l != null && l.name != null)
                {
                    foreach (string dayLight in dayLights)
                    {
                        if (dayLight.Equals(l.name))
                        {
                            l.enabled = isDayMode;
                            break;
                        }
                    }
                }
            }

            foreach (Light l in dayLightsSPH)
            {
                if (l != null && l.name != null)
                {
                    foreach (String dayLight in dayLights)
                    {
                        if (dayLight.Equals(l.name))
                        {
                            l.enabled = isDayMode;
                            break;
                        }
                    }
                }
            }

            // Lights added by the mod
            if (rootGameObject != null)
            {
                Light[] nightLights = rootGameObject.GetComponentsInChildren<Light>();
                foreach (Light l in nightLights)
                {
                    if (l != null)
                    {
                        l.enabled = !isDayMode;
                    }
                }
            }
        }

        private void GenerateNewNightLights()
        {
            DeleteExistingNightLights();

            int lightNameIndex = 0;

            // Floor
            AddLight(new Vector3(15, 0, -15), "Night_Light_" + lightNameIndex++);
            AddLight(new Vector3(15, 0, 15), "Night_Light_" + lightNameIndex++);
            AddLight(new Vector3(-15, 0, -15), "Night_Light_" + lightNameIndex++);
            AddLight(new Vector3(-15, 0, 15), "Night_Light_" + lightNameIndex++);

            // Left Wall
            AddLight(new Vector3(20, 20, 25), "Night_Light_" + lightNameIndex++);
            AddLight(new Vector3(20, 20, -25), "Night_Light_" + lightNameIndex++);
            AddLight(new Vector3(20, 45, 25), "Night_Light_" + lightNameIndex++);
            AddLight(new Vector3(20, 45, -25), "Night_Light_" + lightNameIndex++);

            // Right wall
            AddLight(new Vector3(-20, 20, 25), "Night_Light_" + lightNameIndex++);
            AddLight(new Vector3(-20, 20, -25), "Night_Light_" + lightNameIndex++);
            AddLight(new Vector3(-20, 45, 25), "Night_Light_" + lightNameIndex++);
            AddLight(new Vector3(-20, 45, -25), "Night_Light_" + lightNameIndex++);

            // Back wall
            AddLight(new Vector3(-30, 20, 15), "Night_Light_" + lightNameIndex++);
            AddLight(new Vector3(-30, 20, -15), "Night_Light_" + lightNameIndex++);
            AddLight(new Vector3(-30, 45, 15), "Night_Light_" + lightNameIndex++);
            AddLight(new Vector3(-30, 45, -15), "Night_Light_" + lightNameIndex++);

            // Ceiling (VAB)
            AddLight(new Vector3(-30, 70, 15), "Night_Light_" + lightNameIndex++);
            AddLight(new Vector3(-30, 70, -15), "Night_Light_" + lightNameIndex++);

            AddLight(new Vector3(15, 70, -15), "Night_Light_" + lightNameIndex++);
            AddLight(new Vector3(15, 70, 15), "Night_Light_" + lightNameIndex++);
            AddLight(new Vector3(-15, 70, -15), "Night_Light_" + lightNameIndex++);
            AddLight(new Vector3(-15, 70, 15), "Night_Light_" + lightNameIndex++);
        }

        private void DeleteExistingNightLights()
        {
            // Delete any existing lights
            GameObject[] gameObjectLights = rootGameObject.GetComponentsInChildren<GameObject>();
            foreach (GameObject g in gameObjectLights)
            {
                GameObject.Destroy(g);
            }
        }

        // Helper method for adding additional lights at night
        private void AddLight(Vector3 pos, string name)
        {
            GameObject g = new GameObject("Night_Light_GameObject");
            Light light = g.AddComponent<Light>();

            light.name = name;

            light.enabled = true;
            light.range = 75.0f;
            light.intensity = 0.5f;

            light.shadowStrength = 0.0f;
            light.color = Color.white;

            light.transform.position = pos;
            light.transform.parent = g.transform;

            g.transform.parent = rootGameObject.transform;
        }
    }
}
