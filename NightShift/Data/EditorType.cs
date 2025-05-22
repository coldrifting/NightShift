using System.Collections.Generic;
using NightShift.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NightShift.Data;

public class EditorItemKey<T>(EditorFacility facility, int level, string path) where T : Object
{
    public readonly EditorFacility Facility = facility;
    public readonly int Level = level;
    public readonly string Path = path;

    public GameObject GameObject => Editor.RootInteriorObject?.transform.Find(Path)?.gameObject;
    public Transform Transform => GameObject?.transform;
    public T Component => GameObject ? GameObject?.GetComponent<T>() : null;
}

public static class EditorItemKeyExt
{
    public static void InitializeGameObjects<T>(this List<EditorItemKey<T>> editorTypeList, List<GameObject> list) where T : Object
    {
        list.Clear();
        
        EditorFacility editorType = Editor.Facility;
        int level = Editor.Level;
        
        foreach (EditorItemKey<T> editorItemKey in editorTypeList)
        {
            if (editorItemKey.Facility == editorType && editorItemKey.Level == level)
            {
                GameObject gameObject = editorItemKey.GameObject;
                if (gameObject)
                {
                    list.Add(gameObject);
                }
            }
        }
    }
    
    public static void InitializeTransforms<T>(this List<EditorItemKey<T>> editorTypeList, List<Transform> list) where T : Object
    {
        list.Clear();
        
        EditorFacility editorType = Editor.Facility;
        int level = Editor.Level;
        
        foreach (EditorItemKey<T> editorItemKey in editorTypeList)
        {
            if (editorItemKey.Facility == editorType && editorItemKey.Level == level)
            {
                GameObject gameObject = editorItemKey.GameObject;
                if (gameObject)
                {
                    list.Add(gameObject.transform);
                }
            }
        }
    }
    
    public static void InitializeComponents<T>(this List<EditorItemKey<T>> editorTypeList, List<T> list) where T : Object
    {
        list.Clear();
        
        EditorFacility editorType = Editor.Facility;
        int level = Editor.Level;
        
        foreach (EditorItemKey<T> editorItemKey in editorTypeList)
        {
            if (editorItemKey.Facility == editorType && editorItemKey.Level == level)
            {
                T comp = editorItemKey.Component;
                if (comp)
                {
                    list.Add(comp);
                }
            }
        }
    }
}