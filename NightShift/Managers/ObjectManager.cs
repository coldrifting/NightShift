using System.Collections.Generic;
using NightShift.Data;
using NightShift.Utils;
using UnityEngine;
using Time = NightShift.Utils.Time;

namespace NightShift.Managers;

// Adjusts object visibility and animation playback, and replaces the SPH Lvl 3 Lights with a corrected mesh
public static class ObjectManager
{
    private static readonly List<GameObject> Objects = [];
    private static readonly List<Transform> ObjectReset = [];
    private static readonly List<Animation> ObjectAnim = [];

    public static void Init()
    {
        List<EditorItemKey<GameObject>> allObjectsToHide = 
        [
            new(EditorFacility.SPH, 3, Constants.SphLvl3Kerbals),
            new(EditorFacility.SPH, 3, Constants.SphLvl3MovingTruck1),
            new(EditorFacility.SPH, 3, Constants.SphLvl3MovingTruck2),
            new(EditorFacility.SPH, 3, Constants.SphLvl3MovingTruck3),
            new(EditorFacility.SPH, 3, Constants.SphLvl3MovingTruck4),
            new(EditorFacility.SPH, 2, Constants.KerbalsSph),
            new(EditorFacility.SPH, 1, Constants.KerbalsSph),
            
            new(EditorFacility.VAB, 3, Constants.VabLvl3Kerbals),
            new(EditorFacility.VAB, 3, Constants.VabLvl3ElevatorTruck),
            new(EditorFacility.VAB, 3, Constants.VabLvl3MovingTruck1),
            new(EditorFacility.VAB, 3, Constants.VabLvl3MovingTruck2),
            new(EditorFacility.VAB, 3, Constants.VabLvl3MovingTruck3),
            new(EditorFacility.VAB, 2, Constants.KerbalsVab),
            new(EditorFacility.VAB, 1, Constants.KerbalsVab),
        ];

        List<EditorItemKey<GameObject>> allObjectsToReset =
        [
            new(EditorFacility.VAB, 3, Constants.VabLvl3Elevator1),
            new(EditorFacility.VAB, 3, Constants.VabLvl3Elevator2),
            new(EditorFacility.VAB, 3, Constants.VabLvl3Elevator3),
            new(EditorFacility.VAB, 3, Constants.VabLvl3Elevator4),
        ];

        List<EditorItemKey<Animation>> allAnimationsToStop =
        [
            new(EditorFacility.VAB, 3, Constants.VabLvl3AnimationRoot),
        ];

        allObjectsToHide.InitializeGameObjects(Objects);
        allObjectsToReset.InitializeTransforms(ObjectReset);
        allAnimationsToStop.InitializeComponents(ObjectAnim);
    }

    public static void Apply(double currentTime)
    {
        bool isDay = Time.GetTimeOfDay(currentTime) == TimeOfDay.Day;

        // Enable objects
        foreach (GameObject gameObject in Objects)
        {
            gameObject.SetActive(isDay);
        }

        // Enable Elevator Animations
        foreach (Animation anim in ObjectAnim)
        {
            anim.enabled = isDay;
        }

        foreach (Transform transform in ObjectReset)
        {
            transform.position = Vector3.zero;
        }
    }
}