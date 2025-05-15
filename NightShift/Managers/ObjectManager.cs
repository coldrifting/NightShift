using System.Collections.Generic;
using UnityEngine;

namespace NightShift.Managers;

// Adjusts object visibility, animation playback, and adds new objects when needed
public class ObjectManager : IDayNightManager
{
    private readonly List<GameObject> _objects = [];
    private readonly List<Transform> _objectReset = [];
    private readonly List<Animation> _objectAnim = [];
    
    private GameObject NewSph3Lights { get; set; }

    public ObjectManager()
    {
        Init();
    }
    
    public void Init()
    {
        _objects.Clear();
        _objectReset.Clear();
        _objectAnim.Clear();
        
        var sphLvl3MainBuilding = GameObject.Find("SPHmodern/SPH_interior_modern/SPH_Interior_Geometry/model_sph_interior_main_v16");
        if (sphLvl3MainBuilding)
        {
            NewSph3Lights = GameDatabase.Instance.GetModel("NightShift/Assets/Lights_SPH3");
            NewSph3Lights.name = "SPH_Lights_LVL3";
            NewSph3Lights.transform.parent = sphLvl3MainBuilding.transform;
        }
        
        Transform interiorRoot = Tools.GetRootInteriorTransform();

        if (Tools.IsSph)
        {
            switch (Tools.Level)
            {
                case 3:
                    // SPH - Level 3
                    _objects.TryAddGameObjectFromPath(interiorRoot, "SPH_interior_modern/SPHCrew");
                    _objects.TryAddGameObjectFromPath(interiorRoot, "SPH_interior_modern/SPH_Interior_Geometry/model_props");
                    _objects.TryAddGameObjectFromPath(interiorRoot, "SPH_interior_modern/SPH_Interior_Geometry/model_sph_interior_lights_v16/Component_744_1/rearWindowLightSplashes");
                    _objects.TryAddGameObjectFromPath(interiorRoot, "Lighting_Realtime/Realtime_ExteriorSun");
                    _objects.TryAddGameObjectFromPath(interiorRoot, "Lighting_Realtime/Realtime_SpotlightWindow");
                    
                    break;
                case 2:
                    // SPH - Level 2
                    _objects.TryAddGameObjectFromPath(interiorRoot, "SPHCrew");
                    _objects.TryAddGameObjectFromPath(interiorRoot, "Editors_DayLights/ExteriorSun");
                    break;
                default:
                    // SPH - Level 1
                    _objects.TryAddGameObjectFromPath(interiorRoot, "SPHCrew");
                    _objects.TryAddGameObjectFromPath(interiorRoot, "Editors_DayLights/ExteriorSun");
                    break;
            }
        }
        else
        {
            switch (Tools.Level)
            {
                case 3:
                    // VAB - Level 3
                    _objects.TryAddGameObjectFromPath(interiorRoot, "VAB_interior_modern/VABCrew");
                    _objects.TryAddGameObjectFromPath(interiorRoot, "VAB_interior_modern/VAB_Interior_Geometry/model_props");
                    _objects.TryAddGameObjectFromPath(interiorRoot, "VAB_interior_modern/VAB_Interior_Geometry/model_vab_prop_truck_01");
                    _objects.TryAddGameObjectFromPath(interiorRoot, "VAB_interior_modern/Day Lights/SpotlightSun");

                    _objectReset.TryAddTransformFromPath(interiorRoot, "VAB_interior_modern/VAB_Interior_Geometry/model_vab_elevators/elevator_platform1");
                    _objectReset.TryAddTransformFromPath(interiorRoot, "VAB_interior_modern/VAB_Interior_Geometry/model_vab_elevators/elevator_platform2");
                    _objectReset.TryAddTransformFromPath(interiorRoot, "VAB_interior_modern/VAB_Interior_Geometry/model_vab_elevators/elevator_platform3");
                    _objectReset.TryAddTransformFromPath(interiorRoot, "VAB_interior_modern/VAB_Interior_Geometry/model_vab_elevators/elevator_platform4");
            
                    _objectAnim.TryAddFromPath(interiorRoot, "VAB_interior_modern/VAB_Interior_Geometry");
                    break;
                case 2:
                    // VAB - Level 2
                    _objects.TryAddGameObjectFromPath(interiorRoot, "VABCrew");
                    _objects.TryAddGameObjectFromPath(interiorRoot, "Day Lights/SpotlightSun");
                    break;
                default:
                    // VAB - Level 1
                    _objects.TryAddGameObjectFromPath(interiorRoot, "VABCrew");
                    _objects.TryAddGameObjectFromPath(interiorRoot, "Day Lights/SpotlightSun");
                    break;
            }
        }
    }
    
    public void Apply(TimeOfDay timeOfDay)
    {
        bool isDay = timeOfDay == TimeOfDay.Day;
        
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
            NewSph3Lights.SetActive(Tools.IsSph && Tools.Level == 3);
        }
    }
}