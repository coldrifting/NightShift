using System;

namespace NightShift;

public enum TimeOfDay
{
    Day,
    Twilight,
    Night
}

public static class TimeOfDayExt
{
    public static TimeOfDay Cycle(this TimeOfDay timeOfDay)
    {
        return timeOfDay switch
        {
            TimeOfDay.Day => TimeOfDay.Twilight,
            TimeOfDay.Twilight => TimeOfDay.Night,
            TimeOfDay.Night => TimeOfDay.Day,
            _ => TimeOfDay.Day
        };
    }
}