using NightShift.Utils;
using UnityEngine;

namespace NightShift.Data;

public class MeshTextureInfo(EditorFacility facility, int level, string path, int materialIndex)
{
    public readonly EditorFacility Facility = facility;
    public readonly int Level = level;

    public readonly int MaterialIndex = materialIndex;
    
    public string Id => Facility + "_" + Level;
    
    public GameObject GameObject => Editor.RootInteriorObject?.transform.Find(path)?.gameObject;
    public Transform Transform => GameObject?.transform;
    public MeshRenderer MeshRenderer => GameObject ? GameObject?.GetComponent<MeshRenderer>() : null;
    public Texture Texture => GameDatabase.Instance.GetTexture(Constants.TextureFolder + "/" + Id, false);
}