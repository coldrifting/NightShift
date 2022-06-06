using System;
using System.Collections;
using UnityEngine;

namespace NightShift
{
    internal class TimeManager
    {
        private static CelestialBody sun;

        internal static IEnumerator GetSun()
        {
            while (Planetarium.fetch == null)
            {
                yield return null;
            }
            sun = Planetarium.fetch.Sun;
        }

        internal static bool IsDaytime()
        {
            double localTime = GetLocalTime();
            return (localTime > 0.22 && localTime < 0.78);
        }

        private static double GetLocalTime()
        {
            // Default KSC coordinates
            double lat = -0.0917535863160035;
            double lon = 285.37030688110428;
            CelestialBody body = FlightGlobals.GetHomeBody();

            if (SpaceCenter.Instance != null)
            {
                body = SpaceCenter.Instance.cb;
                lat = SpaceCenter.Instance.Latitude;
                lon = SpaceCenter.Instance.Longitude;
            }
            if (FlightDriver.Pause)
            {
                return 0.5;
            }
            if (sun == null)
            {
                Debug.LogError($"[NightShift] Cannot find local time due to lack of sun");
                return 0.5;
            }
            return Sun.Instance.GetLocalTimeAtPosition(body.GetRelSurfaceNVector(lat, lon), body);
        }
    }
}
