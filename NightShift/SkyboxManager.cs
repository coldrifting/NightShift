using System;
using UnityEngine;

namespace NightShift
{
    // Most of this skybox code comes from the LightsOut mod.
    // Big thanks to that mod's authors for help with making this mod.
    internal class SkyboxManager
    {
        private static Camera mainSceneryCamera;
        private static GameObject skyCamera;
        private static CameraClearFlags originalClearFlags;
        private static int originalCullingMask;

        private static Material DaySky;
        private static Material NightSky;

        internal SkyboxManager()
        {
            mainSceneryCamera = GetSceneryCamera();
            if (mainSceneryCamera == null)
            {
                Debug.Log("[NightShift] Could not find main scenery camera for skybox rotation");
            }

            originalCullingMask = mainSceneryCamera.cullingMask;
            originalClearFlags = mainSceneryCamera.clearFlags;

            DaySky = RenderSettings.skybox;
            NightSky = GenerateNightSkybox();
        }

        internal void EnableDayMode(bool isDaytime)
        {
            if (isDaytime)
            {
                RenderSettings.skybox = DaySky;
                mainSceneryCamera.clearFlags = originalClearFlags;
                mainSceneryCamera.cullingMask = originalCullingMask | 1 << 14;
            }
            else
            {
                RenderSettings.skybox = NightSky;
                mainSceneryCamera.clearFlags = CameraClearFlags.Nothing;
                mainSceneryCamera.cullingMask = originalCullingMask;
            }
        }

        internal void RotateSkybox()
        {
            if (skyCamera)
            {
                skyCamera.transform.position = mainSceneryCamera.transform.position;
                skyCamera.transform.rotation = mainSceneryCamera.transform.rotation;
                // Rotate this correctly for SPH
                if (EditorDriver.editorFacility == EditorFacility.VAB)
                {
                    skyCamera.transform.Rotate(-90, 0, 0, Space.World);
                }
                else if (EditorDriver.editorFacility == EditorFacility.SPH)
                {
                    // Rotation order: Z, X, Y
                    // x rotates about gravity axis
                    // y rotates about sun's rotation axis
                    // z rotates about sun's radial axis (don't know angle)
                    skyCamera.transform.Rotate(-30, 90, -90, Space.World);
                }
            }
        }

        private Material GenerateNightSkybox()
        {
            Material sky = RenderSettings.skybox;
            Material night = new Material(sky);

            skyCamera = new GameObject("NightSkyboxCamera", typeof(Camera));
            skyCamera.GetComponent<Camera>().depth = mainSceneryCamera.depth - 1;
            skyCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
            skyCamera.GetComponent<Camera>().cullingMask = 0;
            originalClearFlags = mainSceneryCamera.clearFlags;

            foreach (Renderer renderer in GalaxyCubeControl.Instance.GetComponentsInChildren<Renderer>())
            {
                Material material = renderer.material;
                Texture texture = material.mainTexture;
                if (texture)
                {
                    switch (renderer.name)
                    {
                        case "ZP":
                            night.SetTexture("_FrontTex", material.mainTexture);
                            break;
                        case "ZN":
                            night.SetTexture("_BackTex", material.mainTexture);
                            break;
                        case "XP":
                            night.SetTexture("_LeftTex", material.mainTexture);
                            break;
                        case "XN":
                            night.SetTexture("_RightTex", material.mainTexture);
                            break;
                        case "YP":
                            night.SetTexture("_UpTex", material.mainTexture);
                            break;
                        case "YN":
                            night.SetTexture("_DownTex", material.mainTexture);
                            break;
                        default:
                            break;
                    }
                }
            }

            skyCamera.AddComponent<Skybox>();
            skyCamera.GetComponent<Skybox>().material = night;

            return night;
        }

        private static Camera GetSceneryCamera()
        {
            Camera[] cameras = UnityEngine.Object.FindObjectsOfType(typeof(Camera)) as Camera[];
            foreach (Camera camera in cameras)
            {
                if (camera.name == "sceneryCam")
                {
                    return camera;
                }
            }
            return null;
        }
    }
}
