using System;
using UnityEngine;

namespace NightShift;

// Most of this skybox code comes from the LightsOut mod.
// Big thanks to that mod's authors for their helpful source code.

// "Replaces" skybox by using a new camera synced to editor camera,
// so we can edit the rotation of the skybox
internal class SkyboxManager : IDayNightManager
{
    private Material _nightSky;
        
    private Camera _nightskyCamera;
    private Camera _sceneryCamera;
        
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private static readonly int UpTex = Shader.PropertyToID("_UpTex");
    private static readonly int BackTex = Shader.PropertyToID("_BackTex");
    private static readonly int LeftTex = Shader.PropertyToID("_LeftTex");
    private static readonly int FrontTex = Shader.PropertyToID("_FrontTex");
    private static readonly int RightTex = Shader.PropertyToID("_RightTex");
    private static readonly int DownTex = Shader.PropertyToID("_DownTex");

    public SkyboxManager()
    {
        Init();
    }

    private static Material BuildSkybox()
    {
        Texture2D[] cube = new Texture2D[6];
        
        MeshRenderer[] renders = GalaxyCubeControl.Instance.transform.GetComponentsInChildren<MeshRenderer>();
        foreach (var render in renders)
        {
            switch (render.name)
            {
                case "YP":
                    cube[0] = render.material.GetTexture(MainTex) as Texture2D;
                    break;
                case "ZN":
                    cube[1] = render.material.GetTexture(MainTex) as Texture2D;
                    break;
                case "XP":
                    cube[2] = render.material.GetTexture(MainTex) as Texture2D;
                    break;
                case "ZP":
                    cube[3] = render.material.GetTexture(MainTex) as Texture2D;
                    break;
                case "XN":
                    cube[4] = render.material.GetTexture(MainTex) as Texture2D;
                    break;
                case "YN":
                    cube[5] = render.material.GetTexture(MainTex) as Texture2D;
                    break;
            }
        }
        
        Material output = new(RenderSettings.skybox.shader)
        {
            name = "Skybox (Night)",
            mainTexture = new Cubemap(cube[0].width, TextureFormat.RGB24, true),
            color = Color.white
        };
        output.SetTexture(UpTex, cube[0]);
        output.SetTexture(BackTex, cube[1]);
        output.SetTexture(LeftTex, cube[2]);
        output.SetTexture(FrontTex, cube[3]);
        output.SetTexture(RightTex, cube[4]);
        output.SetTexture(DownTex, cube[5]);
        
        return output;
    }
    
    public void Init()
    {
        try
        {
            Transform nst = GameObject.Find("NightShift").transform;
            Transform cameraTransform = nst.Find("NightSkyCam");
            if (cameraTransform)
            {
                _nightskyCamera = cameraTransform.GetComponent<Camera>();
            }
            else 
            {
                _nightSky = BuildSkybox();
                _nightskyCamera = new GameObject("NightSkyCam").AddComponent<Camera>();
                _nightskyCamera.transform.parent = GameObject.Find("NightShift").transform;
            }
            
            GameObject sceneryCamGameObject = 
                GameObject.Find("/VAB camera pivot/Main Camera/sceneryCam") ??
                GameObject.Find("/SPH camera pivot/Main Camera/sceneryCam");

            if (!sceneryCamGameObject)
            {
                Debug.Log("[NightShift] Could not find main scenery camera for skybox rotation");
                return;
            }
            
            _sceneryCamera = sceneryCamGameObject.GetComponent<Camera>();
            if (!_sceneryCamera)
                return;
            
            _nightskyCamera.depth = _sceneryCamera.depth - 1;
            _nightskyCamera.clearFlags = CameraClearFlags.Skybox;
            _nightskyCamera.cullingMask = 0;
                
            Skybox skybox = _nightskyCamera.gameObject.AddComponent<Skybox>();
            skybox.material = _nightSky;
        }
        catch (Exception e)
        {
            Debug.Log(e.StackTrace);
            throw;
        }
    }

    public void Switch(bool isDay)
    {
        if (isDay)
        {
            if (!_sceneryCamera) 
                return;
            
            _sceneryCamera.clearFlags = CameraClearFlags.Skybox;
        }
        else
        {
            if (!_sceneryCamera) 
                return;
            
            _sceneryCamera.clearFlags = CameraClearFlags.Nothing;
        }
    }

    public void Update(bool isDay = true)
    {
        if (!_nightskyCamera || isDay) 
            return;

        if (_nightskyCamera is null || _sceneryCamera is null) 
            return;
            
        _nightskyCamera.transform.position = _sceneryCamera.transform.position;
        _nightskyCamera.transform.rotation = _sceneryCamera.transform.rotation;

        // Rotation order: Z, X, Y
        // x rotates about gravity axis
        // y rotates about sun's rotation axis
        // z rotates about sun's radial axis (don't know angle)
        if (EditorDriver.editorFacility == EditorFacility.SPH)
        {
            _nightskyCamera.transform.Rotate(-30, 270, -90, Space.World);
        }
        else
        {
            _nightskyCamera.transform.Rotate(-90, 180, 0, Space.World);
        }
    }
}