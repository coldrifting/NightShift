using System.Collections.Generic;
using NightShift.Data;
using NightShift.Managers.Interface;
using NightShift.Utils;
using UnityEngine;

namespace NightShift.Managers;

// Adjusts object visibility and animation playback, and replaces the SPH Lvl 3 Lights with a corrected mesh
public class ObjectManager : IDayNightManager
{
    private readonly List<GameObject> _objects = [];
    private readonly List<Transform> _objectReset = [];
    private readonly List<Animation> _objectAnim = [];

    private GameObject NewSph3Lights { get; set; }

    public void Init()
    {
        var sphLvl3MainBuilding = GameObject.Find("SPHmodern/" + Constants.SphLvl3InteriorMain);
        if (sphLvl3MainBuilding)
        {
            NewSph3Lights = GameDatabase.Instance.GetModel($"{Constants.ModelFolder}/Lights_SPH3");
            NewSph3Lights.name = "SPH_Lights_LVL3";
            NewSph3Lights.transform.parent = sphLvl3MainBuilding.transform;
        }

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

        allObjectsToHide.InitializeGameObjects(_objects);
        allObjectsToReset.InitializeTransforms(_objectReset);
        allAnimationsToStop.InitializeComponents(_objectAnim);
    }

    public void Apply(double currentTime)
    {
        bool isDay = Time.GetTimeOfDay(currentTime) == TimeOfDay.Day;

        // Enable objects
        foreach (GameObject gameObject in _objects)
        {
            gameObject.SetActive(isDay);
        }

        // Enable Elevator Animations
        foreach (Animation anim in _objectAnim)
        {
            anim.enabled = isDay;
        }

        foreach (Transform transform in _objectReset)
        {
            transform.position = Vector3.zero;
        }
        
        if (NewSph3Lights)
        {
            NewSph3Lights.SetActive(Editor.Facility == EditorFacility.SPH && Editor.Level == 3);
        }
    }
}