using NightShift.Utils;
using UnityEngine;

namespace NightShift.Data;

public class MeshTextureInfo(
    EditorFacility facility, 
    int level, 
    int materialIndex, 
    string path, 
    bool hasLightmap = false, 
    bool hasOverlayTex = false, 
    bool hasEmissiveTex = false, 
    bool hasSkyMaskTex = false, 
    int? overrideLevel = null,
    Color? overrideColor = null)
{
    private const string OverlayTexSuffix = "_Overlay";
    private const string EmissiveTexSuffix = "_Illum";
    private const string SkyMaskSuffix = "_SkyMask";
    
    public readonly EditorFacility Facility = facility;
    public readonly int Level = level;
    
    public readonly int MaterialIndex = materialIndex;

    public readonly bool HasLightmap = hasLightmap;
    public readonly bool HasOverlayTex = hasOverlayTex;
    public readonly bool HasEmissiveTex = hasEmissiveTex;
    public readonly bool HasSkyMaskTex = hasSkyMaskTex;
    
    public readonly Color? OverrideColor = overrideColor;

    private string Id => Facility + "_" + (overrideLevel != null ? overrideLevel.ToString() : Level.ToString());

    private GameObject GameObject => Editor.RootInteriorObject?.transform.Find(path)?.gameObject;
    public MeshRenderer MeshRenderer => GameObject ? GameObject?.GetComponent<MeshRenderer>() : null;
    public Texture OverlayTex => GameDatabase.Instance.GetTexture(Constants.TextureFolder + "/" + Id + OverlayTexSuffix, false);
    public Texture EmissiveTex => GameDatabase.Instance.GetTexture(Constants.TextureFolder + "/" + Id + EmissiveTexSuffix, false);
    public Texture SkyMaskTex => GameDatabase.Instance.GetTexture(Constants.TextureFolder + "/" + Id + SkyMaskSuffix, false);
}