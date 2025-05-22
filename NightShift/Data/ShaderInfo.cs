using UnityEngine;

namespace NightShift.Data;

public class ShaderInfo(Shader dayShader, Shader nightShader)
{
    // Shaders
    public static readonly Shader ShaderDiffuse = Shabby.Shabby.FindShader("KSP/Diffuse");
    public static readonly Shader ShaderSpecular = Shabby.Shabby.FindShader("KSP/Specular");
    public static readonly Shader ShaderSceneryDiffuse = Shabby.Shabby.FindShader("KSP/Scenery/Diffuse");
    public static readonly Shader ShaderScenerySpecular = Shabby.Shabby.FindShader("KSP/Scenery/Specular");
    public static readonly Shader ShaderSceneryBumped = Shabby.Shabby.FindShader("KSP/Scenery/Bumped");
    public static readonly Shader ShaderSceneryBumpedSpecular = Shabby.Shabby.FindShader("KSP/Scenery/Bumped Specular");
    public static readonly Shader ShaderSceneryEmissiveSpecular = Shabby.Shabby.FindShader("KSP/Scenery/Emissive/Specular");
    public static readonly Shader ShaderSceneryGroundKsc = Shabby.Shabby.FindShader("KSP/Scenery/Diffuse Ground KSC Specular");
    public static readonly Shader ShaderLegacyDiffuse = Shabby.Shabby.FindShader("Legacy Shaders/Diffuse");
    public static readonly Shader ShaderSceneryEmissiveLightmapper = Shabby.Shabby.FindShader("KSP/Scenery/Emissive/Lightmapper");
    
    public static readonly Shader ShaderNightShiftDiffuse = Shabby.Shabby.FindShader("NightShift/AmbientOverride/Diffuse");
    public static readonly Shader ShaderNightShiftSpecular = Shabby.Shabby.FindShader("NightShift/AmbientOverride/Specular");
    public static readonly Shader ShaderNightShiftBumped = Shabby.Shabby.FindShader("NightShift/AmbientOverride/Bumped");
    public static readonly Shader ShaderNightShiftBumpedSpecular = Shabby.Shabby.FindShader("NightShift/AmbientOverride/Bumped Specular");
    public static readonly Shader ShaderNightShiftEmissiveSpecular = Shabby.Shabby.FindShader("NightShift/AmbientOverride/Emissive/Specular");
    public static readonly Shader ShaderNightShiftGroundKsc = Shabby.Shabby.FindShader("NightShift/AmbientOverride/Diffuse Ground KSC Specular");
    public static readonly Shader ShaderNightShiftEmissiveLightmapper = Shabby.Shabby.FindShader("NightShift/Overlays/Emissive/Lightmapper");
    
    public static readonly ShaderInfo ShaderMapDiffuse =  new(ShaderSceneryDiffuse, ShaderNightShiftDiffuse);
    public static readonly ShaderInfo ShaderMapSpecular =  new(ShaderScenerySpecular, ShaderNightShiftSpecular);
    public static readonly ShaderInfo ShaderMapEmissiveSpecular =  new(ShaderSceneryEmissiveSpecular, ShaderNightShiftEmissiveSpecular);
    public static readonly ShaderInfo ShaderMapBumped =  new(ShaderSceneryBumped, ShaderNightShiftBumped);
    public static readonly ShaderInfo ShaderMapBumpedSpecular =  new(ShaderSceneryBumpedSpecular, ShaderNightShiftBumpedSpecular);
    public static readonly ShaderInfo ShaderMapGroundKsc =  new(ShaderSceneryGroundKsc, ShaderNightShiftGroundKsc);
    public static readonly ShaderInfo ShaderMapLegacyDiffuse =  new(ShaderLegacyDiffuse, ShaderNightShiftDiffuse);

    public readonly Shader DayShader = dayShader;
    public readonly Shader NightShader = nightShader;
}