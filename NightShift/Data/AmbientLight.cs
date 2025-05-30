using UnityEngine;
using Time = NightShift.Utils.Time;

namespace NightShift.Data;

public static class AmbientLight
{
    private static readonly Color NightColor = new(0.02f, 0.02f, 0.03f);
    private static readonly Color TwilightColor1 = new(0.25f, 0.1f, 0.1f);
    private static readonly Color TwilightColor2 = new(0.75f, 0.55f, 0.3f);
    private static readonly Color DayAmbientColor = new(0.462f, 0.462f, 0.462f);
    private static readonly Color DayFogColor = new(0.24f, 0.24f, 0.15f);
    
    private static readonly Color TwilightSkyColor1 = new(0.06f, 0.03f, 0.0f);
    private static readonly Color TwilightSkyColor2 = new(0.15f, 0.2f, 0.25f);
    private static readonly Color DaySkyColor = new(0.5f, 0.5f, 0.5f);
    
    private static readonly Gradient AmbientLightGradient = new()
    {
        colorKeys =
        [
            new(NightColor, Time.Midnight),
            new(NightColor, Time.NightEnd),
            new(TwilightColor1, Time.Twilight1),
            new(TwilightColor2, Time.Twilight2),
            new(DayAmbientColor, Time.DayStart),
            new(DayAmbientColor, Time.MidDay),
        ],
        alphaKeys =
        [
            new(1, 0),
            new(1, 1),
        ],
        mode = GradientMode.Blend
    };
    
    private static readonly Gradient FogColorGradient = new()
    {
        colorKeys =
        [
            new(NightColor, Time.Midnight),
            new(NightColor, Time.NightEnd),
            new(TwilightColor1, Time.Twilight1),
            new(TwilightColor2, Time.Twilight2),
            new(DayFogColor, Time.DayStart),
            new(DayFogColor, Time.MidDay),
        ],
        alphaKeys =
        [
            new(1, 0),
            new(1, 1),
        ],
        mode = GradientMode.Blend
    };
    
    private static readonly Gradient SkyColorGradient = new()
    {
        colorKeys =
        [
            new(NightColor, Time.Midnight),
            new(NightColor, Time.NightEnd),
            new(TwilightSkyColor1, Time.Twilight1),
            new(TwilightSkyColor2, Time.Twilight2),
            new(DaySkyColor, Time.DayStart),
            new(DaySkyColor, Time.MidDay),
        ],
        alphaKeys =
        [
            new(1, 0),
            new(1, 1),
        ],
        mode = GradientMode.Blend
    };

    // Mirror time input to half gradient entries, since colors should be similar at both dawn and dusk
    public static Color GetAmbientColor(double currentTime)
    {
        double mirroredTime = Time.GetMirroredTime(currentTime);
        return AmbientLightGradient.Evaluate((float)mirroredTime);
    }

    public static Color GetFogColor(double currentTime)
    {
        double mirroredTime = Time.GetMirroredTime(currentTime);
        return FogColorGradient.Evaluate((float)mirroredTime);
    }

    public static float GetLightIntensityFactor(double currentTime)
    {
        double mirroredTime = Time.GetMirroredTime(currentTime);
        return Clamp((float)((mirroredTime - Time.Twilight1) / (Time.DayStart - Time.Twilight1)), 0f, 1f);
    }

    public static Color GetSkyColor(double currentTime)
    {
        double mirroredTime = Time.GetMirroredTime(currentTime);
        return SkyColorGradient.Evaluate((float)mirroredTime);
    }

    private static float Clamp(float value, float min, float max)
    {
        return value < min ? min : value > max ? max : value;
    }
}