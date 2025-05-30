using NightShift.Data;

namespace NightShift.Utils;

public static class Time
{
    public const float Midnight = 0.0f; 
    public const float NightEnd = 0.24f; 
    public const float Twilight1 = 0.25f; 
    public const float Twilight2 = 0.26f; 
    public const float DayStart = 0.28f; 
    public const float MidDay   = 0.5f; 
    
    public static float NextTimeOfDay(double currentTime)
    {
        return GetTimeOfDay(currentTime) switch
        {
            TimeOfDay.Dawn => 0.5f,
            TimeOfDay.Day => 0.75f,
            TimeOfDay.Dusk => 0.0f,
            TimeOfDay.Night => 0.25f,
            _ => 0.71f
        };
    }
        
    public static TimeOfDay GetTimeOfDay(double currentTime)
    {
        return currentTime switch
        {
            >= 0.22f and < 0.30f => TimeOfDay.Dawn,
            >= 0.30f and < 0.70f => TimeOfDay.Day,
            >= 0.70f and < 0.78f => TimeOfDay.Dusk,
            <  0.22f or  > 0.78f => TimeOfDay.Night,
            _ => TimeOfDay.Day
        };
    }

    public static double GetMirroredTime(double currentTime)
    {
        return currentTime > 0.5 ? 1.0 - currentTime : currentTime;
    }
    
    public static double GetTime()
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
            return (float)Sun.Instance.GetLocalTimeAtPosition(lat, lon, body);
        }
            
        //NightShift.Warn($"Cannot find local time: can not find solar system main sun");
        return 0.5;
    }
}