using System.Collections.Generic;
using UnityEngine;

namespace NightShift.Utils;

public static class Tools
{
    public static GameObject TryGetGameObjectFromPath(this Transform t, string path)
    {
        Transform tChild = t.Find(path);
        return !tChild 
            ? null 
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
            return null;
        
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

    public static List<T> GetAllChildComponents<T>(this Transform t) where T : Component
    {
        List<T> all = [];
        
        Queue<Transform> queue = new();
        queue.Enqueue(t);
        while (queue.Count > 0)
        {
            Transform tChild = queue.Dequeue();

            T comp = tChild.GetComponent<T>();
            if (comp)
            {
                all.Add(comp);
            }
            
            foreach (Transform child in tChild)
            {
                queue.Enqueue(child);
            }
        }

        return all;
    }
}