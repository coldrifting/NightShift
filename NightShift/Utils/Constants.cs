using UnityEngine;

namespace NightShift.Utils;

public static class Constants
{
    // Asset Paths
    public const string TextureFolder = "NightShift/Assets/Textures";
    public const string ModelFolder = "NightShift/Assets/Models";
    
    private const string IconFolder = "NightShift/Assets/Icons";
    
    public static readonly Texture2D AppIconDawn = GameDatabase.Instance.GetTexture($"{IconFolder}/Dawn", false);
    public static readonly Texture2D AppIconDay = GameDatabase.Instance.GetTexture($"{IconFolder}/Day", false);
    public static readonly Texture2D AppIconDusk = GameDatabase.Instance.GetTexture($"{IconFolder}/Dusk", false);
    public static readonly Texture2D AppIconNight = GameDatabase.Instance.GetTexture($"{IconFolder}/Night", false);
    
    // The below paths do not include the root scenery object
    
    // Animation And Hiding Object Targets
    public const string KerbalsSph = "SPHCrew";
    public const string KerbalsVab = "VABCrew";
    
    public const string SphLvl3Kerbals = "SPH_interior_modern/SPHCrew";
    public const string SphLvl3MovingTruck1 = "SPH_interior_modern/SPH_Interior_Geometry/model_props/model_prop_truck_h02";
    public const string SphLvl3MovingTruck2 = "SPH_interior_modern/SPH_Interior_Geometry/model_props/model_prop_truck_h03";
    public const string SphLvl3MovingTruck3 = "SPH_interior_modern/SPH_Interior_Geometry/model_props/model_prop_truck_m01";
    public const string SphLvl3MovingTruck4 = "SPH_interior_modern/SPH_Interior_Geometry/model_props/model_prop_truck_m02";
    
    public const string VabLvl3Kerbals = "VAB_interior_modern/VABCrew";
    public const string VabLvl3Elevator1 = "VAB_interior_modern/VAB_Interior_Geometry/model_vab_elevators/elevator_platform1";
    public const string VabLvl3Elevator2 = "VAB_interior_modern/VAB_Interior_Geometry/model_vab_elevators/elevator_platform2";
    public const string VabLvl3Elevator3 = "VAB_interior_modern/VAB_Interior_Geometry/model_vab_elevators/elevator_platform3";
    public const string VabLvl3Elevator4 = "VAB_interior_modern/VAB_Interior_Geometry/model_vab_elevators/elevator_platform4";
    
    public const string VabLvl3AnimationRoot = "VAB_interior_modern/VAB_Interior_Geometry";
    public const string VabLvl3ElevatorTruck = "VAB_interior_modern/VAB_Interior_Geometry/model_vab_prop_truck_01";
    public const string VabLvl3MovingTruck1 = "VAB_interior_modern/VAB_Interior_Geometry/model_props/model_prop_truck_h02";
    public const string VabLvl3MovingTruck2 = "VAB_interior_modern/VAB_Interior_Geometry/model_props/model_prop_truck_h03";
    public const string VabLvl3MovingTruck3 = "VAB_interior_modern/VAB_Interior_Geometry/model_props/model_prop_truck_m02";

    // Texture Replacement Target Objects
    public const string SphLvl3InteriorMain = "SPH_interior_modern/SPH_Interior_Geometry/model_sph_interior_main_v16";
    public const string SphLvl3SideGates = "SPH_interior_modern/SPH_Interior_Geometry/model_sph_interior_gates_v16/Component_755_1/Mesh1";
    public const string SphLvl2Windows = "SPH_2_interior/SPH_2_windows";
    public const string SphLvl2Door1 = "SPH_2_interior/SPH_2_door1";
    public const string SphLvl2Door2 = "SPH_2_interior/SPH_2_door2";
    public const string SphLvl2Door3 = "SPH_2_interior/SPH_2_door3";
    public const string SphLvl2Door4 = "SPH_2_interior/SPH_2_door4";
    public const string SphLvl1Window = "SPH-1_Interior/group37_polySurface813";

    public const string VabLvl3Windows = "VAB_interior_modern/VAB_Interior_Geometry/model_vab_windows";
    public const string VabLvl3Walls = "VAB_interior_modern/VAB_Interior_Geometry/model_vab_interior_walls_v17";
    public const string VabLvl1Windows = "VAB_lev1_interior/INTERIOR_VAB_lev2 1/INTERIOR_VAB_lev2 1_MeshPart1";
}