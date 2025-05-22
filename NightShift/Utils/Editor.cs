using UnityEngine;

namespace NightShift.Utils;

public class Editor
{
    public static EditorFacility Facility => EditorDriver.editorFacility;
    public static int Level => GetEditorLevel();
    
    private static int GetEditorLevel()
    {
        return EditorDriver.editorFacility switch
        {
            EditorFacility.SPH => (int)(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar) * 2) + 1,
            EditorFacility.VAB => (int)(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding) * 2) + 1,
            _ => 3
        };
    }

    public static GameObject RootInteriorObject =>
        (Facility, Level) switch
        {
            (EditorFacility.SPH, 1) => GameObject.Find("Sphlvl1"),
            (EditorFacility.SPH, 2) => GameObject.Find("SPHlvl2"),
            (EditorFacility.VAB, 1) => GameObject.Find("Vablvl2"), // Why Squad?
            (EditorFacility.VAB, 2) => GameObject.Find("VABlvl3"),
            _ => Facility == EditorFacility.SPH
                ? GameObject.Find("SPHmodern")
                : GameObject.Find("VABmodern")
        };

    public static GameObject RootExteriorObject =>
        Facility == EditorFacility.SPH 
            ? GameObject.Find("SPHscenery") 
            : GameObject.Find("VABscenery");
}