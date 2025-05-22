using UnityEngine;

namespace NightShift.Data;

public static class AmbientLight
{
    private const float Midnight = 0.0f; 
    private const float NightEnd = 0.24f; 
    private const float Twilight1 = 0.25f; 
    private const float Twilight2 = 0.26f; 
    private const float DayStart = 0.28f; 
    private const float MidDay   = 0.5f; 
    
    private static readonly Color NightColor = new(0.02f, 0.02f, 0.03f);
    private static readonly Color TwilightColor1 = new(0.25f, 0.1f, 0.1f);
    private static readonly Color TwilightColor2 = new(0.75f, 0.55f, 0.3f);
    private static readonly Color DayAmbientColor = new(0.462f, 0.462f, 0.462f);
    private static readonly Color DayFogColor = new(0.24f, 0.24f, 0.15f);

    private static readonly Gradient AmbientLightGradient = new()
    {
        colorKeys =
        [
            new(NightColor, Midnight),
            new(NightColor, NightEnd),
            new(TwilightColor1, Twilight1),
            new(TwilightColor2, Twilight2),
            new(DayAmbientColor, DayStart),
            new(DayAmbientColor, MidDay),
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
            new(NightColor, Midnight),
            new(NightColor, NightEnd),
            new(TwilightColor1, Twilight1),
            new(TwilightColor2, Twilight2),
            new(DayFogColor, DayStart),
            new(DayFogColor, MidDay),
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
        double mirroredTime = currentTime > 0.5 ? 1.0 - currentTime : currentTime;
        return AmbientLightGradient.Evaluate((float)mirroredTime);
    }

    public static Color GetFogColor(double currentTime)
    {
        double mirroredTime = currentTime > 0.5 ? 1.0 - currentTime : currentTime;
        return FogColorGradient.Evaluate((float)mirroredTime);
    }

    public static float GetLightIntensityFactor(double currentTime)
    {
        double mirroredTime = currentTime > 0.5 ? 1.0 - currentTime : currentTime;
        return Clamp((float)((mirroredTime - Twilight1) / (DayStart - Twilight1)), 0f, 1f);
    }

    private static float Clamp(float value, float min, float max)
    {
        return value < min ? min : value > max ? max : value;
    }
}