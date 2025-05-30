using UnityEngine;

namespace NightShift.Data;

public class ShaderInfo(Shader dayShader, Shader nightShader)
{
    // Shaders
    private static readonly Shader ShaderAlphaCutoff = Shabby.Shabby.FindShader("KSP/Alpha/Cutoff");
    private static readonly Shader ShaderAlphaCutoffBumped = Shabby.Shabby.FindShader("KSP/Alpha/Cutoff Bumped");
    private static readonly Shader ShaderSceneryFlag = Shabby.Shabby.FindShader("KSP/Scenery/Flag");
    private static readonly Shader ShaderSceneryDecalBlended = Shabby.Shabby.FindShader("KSP/Scenery/Decal/Blended");
    private static readonly Shader ShaderSceneryDecalMultiply = Shabby.Shabby.FindShader("KSP/Scenery/Decal/Multiply");
    private static readonly Shader ShaderSceneryDiffuse = Shabby.Shabby.FindShader("KSP/Scenery/Diffuse");
    private static readonly Shader ShaderSceneryDiffuseDetail = Shabby.Shabby.FindShader("KSP/Scenery/Diffuse Detail");
    private static readonly Shader ShaderSceneryDiffuseMultiply = Shabby.Shabby.FindShader("KSP/Scenery/Diffuse Multiply");
    private static readonly Shader ShaderScenerySpecular = Shabby.Shabby.FindShader("KSP/Scenery/Specular");
    private static readonly Shader ShaderSceneryBumped = Shabby.Shabby.FindShader("KSP/Scenery/Bumped");
    private static readonly Shader ShaderSceneryBumpedSpecular = Shabby.Shabby.FindShader("KSP/Scenery/Bumped Specular");
    private static readonly Shader ShaderSceneryEmissiveDiffuse = Shabby.Shabby.FindShader("KSP/Scenery/Emissive/Diffuse");
    private static readonly Shader ShaderSceneryEmissiveSpecular = Shabby.Shabby.FindShader("KSP/Scenery/Emissive/Specular");
    private static readonly Shader ShaderSceneryGroundKsc = Shabby.Shabby.FindShader("KSP/Scenery/Diffuse Ground KSC Specular");
    private static readonly Shader ShaderLegacyDiffuse = Shabby.Shabby.FindShader("Legacy Shaders/Diffuse");
    private static readonly Shader ShaderLegacyBumpedDiffuse = Shabby.Shabby.FindShader("Legacy Shaders/Bumped Diffuse");
    private static readonly Shader ShaderLegacyTransparentDiffuse = Shabby.Shabby.FindShader("Legacy Shaders/Transparent/Diffuse");
    
    private static readonly Shader ShaderNightShiftKsc = Shabby.Shabby.FindShader("NightShift/AmbientOverride/KSC");
    private static readonly Shader ShaderNightShiftAlphaCutoff = Shabby.Shabby.FindShader("NightShift/AmbientOverride/AlphaCutoff");
    private static readonly Shader ShaderNightShiftFlag = Shabby.Shabby.FindShader("NightShift/AmbientOverride/Flag");
    private static readonly Shader ShaderNightShiftDiffuse = Shabby.Shabby.FindShader("NightShift/AmbientOverride/Diffuse");
    private static readonly Shader ShaderNightShiftBumped = Shabby.Shabby.FindShader("NightShift/AmbientOverride/Bumped");
    private static readonly Shader ShaderNightShiftEmissive = Shabby.Shabby.FindShader("NightShift/AmbientOverride/Emissive");
    
    public static readonly Shader ShaderNightShiftOverlaysEmissiveLightmapper = Shabby.Shabby.FindShader("NightShift/Overlays/EmissiveLightmapper");
    
    // Shader mappings
    public static readonly ShaderInfo ShaderMapAlphaCutoff =    new(ShaderAlphaCutoff,   ShaderNightShiftAlphaCutoff);
    public static readonly ShaderInfo ShaderMapAlphaCutoffBumped =    new(ShaderAlphaCutoffBumped,   ShaderNightShiftAlphaCutoff);
    public static readonly ShaderInfo ShaderMapLegacyDiffuse =    new(ShaderLegacyDiffuse,   ShaderNightShiftDiffuse);
    public static readonly ShaderInfo ShaderMapLegacyBumpedDiffuse =    new(ShaderLegacyBumpedDiffuse,   ShaderNightShiftDiffuse);
    public static readonly ShaderInfo ShaderMapLegacyTransparentDiffuse =    new(ShaderLegacyTransparentDiffuse,   ShaderNightShiftAlphaCutoff);
    public static readonly ShaderInfo ShaderMapDecalBlended =     new(ShaderSceneryDecalBlended,   ShaderNightShiftDiffuse);
    public static readonly ShaderInfo ShaderMapDecalMultiply =     new(ShaderSceneryDecalMultiply,   ShaderNightShiftDiffuse);
    public static readonly ShaderInfo ShaderMapDiffuseMultiply =     new(ShaderSceneryDiffuseMultiply,   ShaderNightShiftDiffuse);
    public static readonly ShaderInfo ShaderMapDiffuseDetail =     new(ShaderSceneryDiffuseDetail,   ShaderNightShiftDiffuse);
    public static readonly ShaderInfo ShaderMapDiffuse =          new(ShaderSceneryDiffuse,  ShaderNightShiftDiffuse);
    public static readonly ShaderInfo ShaderMapSpecular =         new(ShaderScenerySpecular, ShaderNightShiftDiffuse);
    public static readonly ShaderInfo ShaderMapFlag =          new(ShaderSceneryFlag,  ShaderNightShiftFlag);
    public static readonly ShaderInfo ShaderMapBumped =           new(ShaderSceneryBumped,           ShaderNightShiftBumped);
    public static readonly ShaderInfo ShaderMapBumpedSpecular =   new(ShaderSceneryBumpedSpecular,   ShaderNightShiftBumped);
    public static readonly ShaderInfo ShaderMapEmissiveDiffuse = new(ShaderSceneryEmissiveDiffuse, ShaderNightShiftEmissive);
    public static readonly ShaderInfo ShaderMapEmissiveSpecular = new(ShaderSceneryEmissiveSpecular, ShaderNightShiftEmissive);
    
    public static readonly ShaderInfo ShaderMapGroundKsc =        new(ShaderSceneryGroundKsc, ShaderNightShiftKsc);

    public readonly Shader DayShader = dayShader;
    public readonly Shader NightShader = nightShader;
}