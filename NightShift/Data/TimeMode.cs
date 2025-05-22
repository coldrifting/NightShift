using System;

namespace NightShift.Data;

public enum TimeMode
{
    ReReWind,
    Rewind,
    Back,
    Pause,
    Forward,
    FastForward,
    FastFastForward,
}

public static class TimeModeExt
{
    public static T Prev<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");

        T[] arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf(arr, src) - 1;
        return (-1==j) ? arr[0] : arr[j];            
    }
    
    public static T Next<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");

        T[] arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf(arr, src) + 1;
        return (arr.Length==j) ? arr[arr.Length - 1] : arr[j];            
    }
}