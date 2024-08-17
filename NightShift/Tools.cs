using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Upgradeables;

namespace NightShift;

public static class Tools
{
    
    
    public static bool IsSph => EditorDriver.editorFacility == EditorFacility.SPH;
    public static int Level => GetEditorLevel();

    private static int GetEditorLevel()
    {
        Transform editorRoot = GetRootInteriorTransform();
        if (!editorRoot)
            return 3;

        return editorRoot.name switch
        {
            "SPHmodern" or "VABmodern" => 3,
            "SPHlvl1" or "VABlvl2" => 1,
            "SPHlvl2" or "VABlvl3" => 2,
            _ => 3
        };
    }

    public static bool IsDaytime => GetLocalTime() is > 0.22 and < 0.78;

    private static double GetLocalTime()
    {
        if (FlightDriver.Pause)
        {
            return 0.5;
        }

        double lon;
        double lat;
        CelestialBody body;

        if (SpaceCenter.Instance)
        {
            lon = SpaceCenter.Instance.Longitude;
            lat = SpaceCenter.Instance.Latitude;
            body = SpaceCenter.Instance.cb;
        }
        else
        {
            // Default KSC coordinates
            lon = 285.37030688110428;
            lat = -0.0917535863160035;
            body = FlightGlobals.GetHomeBody();
        }

        if (Sun.Instance)
        {
            return Sun.Instance.GetLocalTimeAtPosition(body.GetRelSurfaceNVector(lat, lon), body);
        }
            
        Debug.LogError($"[NightShift] Cannot find local time: can not find solar system main sun");
        return 0.5;
    }
    
    public static Transform GetRootInteriorTransform()
    {
        EditorDriver ed = EditorDriver.fetch;

        FieldInfo fi = typeof(EditorDriver).GetField("interiorScene", BindingFlags.NonPublic | BindingFlags.Instance);
        if (fi == null) 
            return null;
        
        UpgradeableInteriorScene x = fi.GetValue(ed) as UpgradeableInteriorScene;
        FieldInfo fi2 = typeof(UpgradeableInteriorScene).GetField("loadedSceneObject",
            BindingFlags.NonPublic | BindingFlags.Instance);
        if (fi2 == null) 
            return null;
        
        GameObject gx = fi2.GetValue(x) as GameObject;
        return gx
            ? gx.transform 
            : null;
    }

    public static Transform GetRootExteriorTransform()
    {
        EditorDriver ed = EditorDriver.fetch;

        FieldInfo fi = typeof(EditorDriver).GetField("editorSceneryRoot", BindingFlags.NonPublic | BindingFlags.Instance);
        if (fi == null) 
            return null;

        return fi.GetValue(ed) is not Transform x 
            ? null 
            : x;
    }

    public static GameObject TryGetGameObjectFromPath(this Transform t, string path)
    {
        Transform tChild = t.Find(path);
        return !tChild 
            ? default 
            : tChild.gameObject;
    }
    
    public static void TryAddGameObjectFromPath(this List<GameObject> l, Transform t, string path)
    {
        GameObject g = TryGetGameObjectFromPath(t, path);
        if (g is not null)
        {
            l.Add(g);
        }
    }
    
    public static void TryAddTransformFromPath(this List<Transform> l, Transform t, string path)
    {
        GameObject g = TryGetGameObjectFromPath(t, path);
        if (g is not null)
        {
            l.Add(g.transform);
        }
    }

    public static T TryGetCompFromPath<T>(this Transform t, string path) where T: Component
    {
        Transform tChild = t.Find(path);
        if (!tChild) 
            return default;
        
        T comp = tChild.GetComponent<T>();
        return comp;
    }
    
    public static void TryAddFromPath<T>(this List<T> l, Transform t, string path) where T: Component
    {
        T comp = TryGetCompFromPath<T>(t, path);
        if (comp is not null)
        {
            l.Add(comp);
        }
    }
}