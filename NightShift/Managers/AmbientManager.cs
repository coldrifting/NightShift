using System.Collections.Generic;
using NightShift.Data;
using NightShift.Utils;
using UnityEngine;

namespace NightShift.Managers;

// Replaces shaders for outside objects for fine-tuned control of ambient light colors
public static class AmbientManager
{
    private static readonly int AmbientLightOverride = Shader.PropertyToID("_AmbientLightOverride");
    
    private static readonly Dictionary<string, ShaderInfo> MaterialInfoSkinnedMesh = new()
    {
	    {"SPHscenery/SpaceCenter/LaunchPad/Facility/Flag/flagPole_lev1/model01/Flag", ShaderInfo.ShaderMapFlag},
	    {"SPHscenery/SpaceCenter/LaunchPad/Facility/Flag/flagPole_lev1/model01/Rope", ShaderInfo.ShaderMapFlag},
	    {"SPHscenery/SpaceCenter/LaunchPad/Facility/LaunchPadMedium/KSCFlagPoleLaunchPad/Building/model01/Flag", ShaderInfo.ShaderMapFlag},
	    {"SPHscenery/SpaceCenter/LaunchPad/Facility/LaunchPadMedium/KSCFlagPoleLaunchPad/Building/model01/Rope", ShaderInfo.ShaderMapFlag},
	    {"VABscenery/SpaceCenter/LaunchPad/Facility/Flag/flagPole_lev1/model01/Flag", ShaderInfo.ShaderMapFlag},
	    {"VABscenery/SpaceCenter/LaunchPad/Facility/Flag/flagPole_lev1/model01/Rope", ShaderInfo.ShaderMapFlag},
	    {"VABscenery/SpaceCenter/LaunchPad/Facility/LaunchPadMedium/KSCFlagPoleLaunchPad/Building/model01/Flag", ShaderInfo.ShaderMapFlag},
	    {"VABscenery/SpaceCenter/LaunchPad/Facility/LaunchPadMedium/KSCFlagPoleLaunchPad/Building/model01/Rope", ShaderInfo.ShaderMapFlag},
    };
    
    private static readonly Dictionary<string, ShaderInfo> MaterialInfo = new()
    {
		{"ksp_pad_launchpad_grate_alpha", ShaderInfo.ShaderMapLegacyTransparentDiffuse},
		{"apollo_cinderblocks_01", ShaderInfo.ShaderMapDiffuse},
		{"apollo_cinderblocks_02", ShaderInfo.ShaderMapBumped},
		{"apollo_dark_blocks_01", ShaderInfo.ShaderMapDiffuse},
		{"apollo_light_blocks_01", ShaderInfo.ShaderMapBumped},
		{"apollo_roll_up_door_01", ShaderInfo.ShaderMapDiffuse},
		{"old_metal_plates_01_cm", ShaderInfo.ShaderMapDiffuse},
		{"apollo_corrugated_metal_01", ShaderInfo.ShaderMapDiffuse},
		{"apollo_corrugated_metal_02", ShaderInfo.ShaderMapDiffuse},
		{"apollo_roof", ShaderInfo.ShaderMapBumped},
		{"Apollo_windows_and_doors", ShaderInfo.ShaderMapEmissiveSpecular},
		{"apollo_windows_doors_02", ShaderInfo.ShaderMapEmissiveSpecular},
		{"asphalt_holeA", ShaderInfo.ShaderMapGroundKsc},
		{"asphalt_transition", ShaderInfo.ShaderMapGroundKsc},
		{"bluePlates", ShaderInfo.ShaderMapDiffuse},
		{"ConcretePatch", ShaderInfo.ShaderMapAlphaCutoffBumped},
		{"ConcretePatch2", ShaderInfo.ShaderMapAlphaCutoffBumped},
		{"ConcretePatch3", ShaderInfo.ShaderMapAlphaCutoff},
		{"ConcretePatch4", ShaderInfo.ShaderMapAlphaCutoff},
		{"corrugatedMetal", ShaderInfo.ShaderMapDiffuse},
		{"crater", ShaderInfo.ShaderMapLegacyDiffuse},
		{"darkGreyPlates", ShaderInfo.ShaderMapDiffuse},
		{"darkTerrain_fade", ShaderInfo.ShaderMapGroundKsc},
		{"default", ShaderInfo.ShaderMapDiffuseDetail},
		{"desert groundTransition", ShaderInfo.ShaderMapGroundKsc},
		{"DesertPlanetRoverFloorLocalSpace", ShaderInfo.ShaderMapGroundKsc},
		{"dirt_road", ShaderInfo.ShaderMapDiffuse},
		{"dirt_road_end", ShaderInfo.ShaderMapDiffuseMultiply},
		{"dirt_transition", ShaderInfo.ShaderMapDiffuse},
		{"door", ShaderInfo.ShaderMapDiffuse},
		{"flagPole_lev1", ShaderInfo.ShaderMapDiffuse},
		{"FlowerBowlCrater", ShaderInfo.ShaderMapGroundKsc},
		{"gasTower", ShaderInfo.ShaderMapDiffuse},
		{"greyPlates", ShaderInfo.ShaderMapDiffuse},
		{"kap_pad_launchPad_mat", ShaderInfo.ShaderMapBumpedSpecular},
		{"ksc_exterior_terrain_asphalt", ShaderInfo.ShaderMapDiffuse},
		{"ksc_exterior_terrain_asphalt_diffuse", ShaderInfo.ShaderMapDiffuse},
		{"ksc_exterior_terrain_asphalt_line", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_asphalt_runwayLVL01", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_asphaltDark", ShaderInfo.ShaderMapDiffuseMultiply},
		{"ksc_exterior_terrain_grass", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_02", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_ACLvl01", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_AClvl02", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_AClvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_adminLvl01", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_adminLvl02", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_adminLvl02_groundTransition", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_adminLvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_airfieldIsland_groundTransition", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_crawlerway", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_crawlerwayGround", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_crawlerwayGroundLvl01", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_crawlerwayGroundLvl02", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_crawlerwayLvl01", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_crawlerwayLvl02", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_fillerX1Lvl01", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_fillerx1Lvl01", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_fillerx1Lvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_fillerx1lvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_fillerX2Lvl01", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_fillerx2Lvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_fillerx2lvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_launchPadLvl01", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_launchPadLvl01_darkTerrainFade", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_launchPadLvl02", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_launchPadLvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_launchPadLvl0302", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_launchPadLvl03_", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_launchPadLvl03_02", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_MCLvl01", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_MCLvl02", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_MCLvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_MClvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_r&dLevel03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_RnDLvl01", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_RnDLvl02", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_runway", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_runwayEnd09", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_runwayEnd09Lvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_runwayEnd27", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_runwayEnd27Lvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_runwayLvl01", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_runwayLvl02_asphaltTransition", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_runwayLvl02_groundPlane", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_runwayLvl02_groundTransition", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_runwayLvl02GoundPlane", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_runwaySec01", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_runwaySec01Lvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_runwaySec02", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_runwaySec02Lvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_runwaySec03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_runwaySec03Lvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_runwaySec04", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_runwaySec04Lvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_sphLvl01", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_SPHLvl01", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_sphLvl02", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_SPHLvl02", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_SPHlvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_sphLvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_TSLvl01", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_TSLvl02", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_TSLvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_TSlvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_vabLvl01", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_VABLvl01", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_vabLvl02", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_VABLvl02", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_VABLvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_grass_vabLvl03", ShaderInfo.ShaderMapGroundKsc},
		{"ksc_exterior_terrain_ground", ShaderInfo.ShaderMapDiffuse},
		{"ksc_exterior_terrain_tile_01_diffuse", ShaderInfo.ShaderMapDiffuse},
		{"ksc_terrain_TX", ShaderInfo.ShaderMapLegacyDiffuse},
		{"ksc_Terrain_TX", ShaderInfo.ShaderMapLegacyDiffuse},
		{"ksp_pad_concrete_mat", ShaderInfo.ShaderMapDiffuse},
		{"ksp_pad_cylTank_mat", ShaderInfo.ShaderMapBumpedSpecular},
		{"ksp_pad_pipes_mat", ShaderInfo.ShaderMapBumpedSpecular},
		{"ksp_pad_sphereTank_mat", ShaderInfo.ShaderMapSpecular},
		{"ksp_pad_waterTower_mat", ShaderInfo.ShaderMapBumpedSpecular},
		{"launchPad_lines", ShaderInfo.ShaderMapDiffuse},
		{"launchPad_main", ShaderInfo.ShaderMapDiffuse},
		{"metalPlates_blue", ShaderInfo.ShaderMapBumped},
		{"metalPlates_brown", ShaderInfo.ShaderMapDiffuse},
		{"MetalTile01", ShaderInfo.ShaderMapBumped},
		{"MetalTile02", ShaderInfo.ShaderMapBumped},
		{"model_scaffolding_01", ShaderInfo.ShaderMapDiffuse},
		{"model_tracking_exterior", ShaderInfo.ShaderMapSpecular},
		{"model_vab_interior_props", ShaderInfo.ShaderMapSpecular},
		{"model_vab_exterior_tile_01", ShaderInfo.ShaderMapSpecular},
		{"model_vab_exterior_details", ShaderInfo.ShaderMapEmissiveSpecular},
		{"model_vab_exterior_details_01", ShaderInfo.ShaderMapEmissiveSpecular},
		{"model_vab_exterior_lines", ShaderInfo.ShaderMapSpecular},
		{"model_vab_exterior_lines_01", ShaderInfo.ShaderMapDiffuse},
		{"model_vab_exterior_props", ShaderInfo.ShaderMapSpecular},
		{"model_vab_exterior_props_01", ShaderInfo.ShaderMapDiffuse},
		{"model_vab_exterior_tile", ShaderInfo.ShaderMapSpecular},
		{"model_vab_exterior_tile_00", ShaderInfo.ShaderMapSpecular},
		{"model_vab_exterior_tile_04_roof", ShaderInfo.ShaderMapDiffuse},
		{"model_vab_exterior_tile_SEMI_PAINTED_00", ShaderInfo.ShaderMapDiffuse},
		{"model_vab_exterior_tile_SEMI_PAINTED_01", ShaderInfo.ShaderMapDiffuse},
		{"model_vab_exterior_lines_01_decal", ShaderInfo.ShaderMapDiffuse},
		{"outUV_telescopios_", ShaderInfo.ShaderMapDiffuse},
		{"podMemorialBase", ShaderInfo.ShaderMapBumpedSpecular},
		{"podMemorialPod", ShaderInfo.ShaderMapBumpedSpecular},
		{"Pole", ShaderInfo.ShaderMapSpecular},
		{"model_tracking_exterior_01", ShaderInfo.ShaderMapSpecular},
		{"props", ShaderInfo.ShaderMapDiffuse},
		{"props2", ShaderInfo.ShaderMapDiffuse},
		{"props3", ShaderInfo.ShaderMapDiffuse},
		{"props_apollo_01", ShaderInfo.ShaderMapDiffuse},
		{"props_apollo_02", ShaderInfo.ShaderMapDiffuse},
		{"RnD_green_cinderblocks_and_roof_tile", ShaderInfo.ShaderMapBumped},
		{"Rust", ShaderInfo.ShaderMapDecalMultiply},
		{"Rust_wide", ShaderInfo.ShaderMapDecalMultiply},
		{"snowmanFloor", ShaderInfo.ShaderMapGroundKsc},
		{"snowmanFloor_highQuality", ShaderInfo.ShaderMapGroundKsc},
		{"sph_apollo_interior_ground", ShaderInfo.ShaderMapDiffuse},
		{"SportsCarCrater", ShaderInfo.ShaderMapGroundKsc},
		{"SputnikCrater", ShaderInfo.ShaderMapGroundKsc},
		{"vab_1_interior_props3", ShaderInfo.ShaderMapDiffuse},
		{"vab_2_exterior_props4", ShaderInfo.ShaderMapDiffuse},
		{"vab_2_interior_props1", ShaderInfo.ShaderMapDiffuse},
		{"vab_2_interior_props2", ShaderInfo.ShaderMapDiffuse},
		{"vab_2_interior_windows1", ShaderInfo.ShaderMapEmissiveDiffuse},
		{"vab_exterior_lines_apollo", ShaderInfo.ShaderMapDiffuse},
		{"Wall Dirt", ShaderInfo.ShaderMapDecalMultiply},
		{"White_Concrete_blocks", ShaderInfo.ShaderMapLegacyBumpedDiffuse},
		{"Window_gunk", ShaderInfo.ShaderMapDecalMultiply},
		{"windows_props_apollo", ShaderInfo.ShaderMapDiffuse},
		{"BurnMark", ShaderInfo.ShaderMapAlphaTranslucent},
    };

    private static readonly List<Material> Materials = [];

    private static readonly List<string> InsideMeshNames =
    [
	    "SPHlvl1/SPH_lev1_ground",
	    "SPHlvl1/SPH-1_Interior/SPH_1_concrete_platform", // sph_apollo_interior_ground Only
	    "VABlvl2/VAB_lev1_interior/VAB_lev1_groundPlane",
	    "SPHlvl2/SPH_lev2_groundPlane",
	    "VABlvl3/VAB_lev2_groundPlane",
	    "SPHmodern/SPH_interior_modern/model_sph_exterior_ground_v46n",
	    "VABmodern/VAB_interior_modern/model_vab_exterior_ground_v46n",
	    "VABmodern/VAB_interior_modern/ksp_pad_cylTank/obj_tank",
	    "VABmodern/VAB_interior_modern/ksp_pad_cylTank/Tank Base",
	    "SPHmodern/SPH_interior_modern/SPH_Interior_Geometry/model_props/model_prop_truck_h01/Mesh1",
	    "VABmodern/VAB_interior_modern/VAB_Interior_Geometry/model_props/model_prop_truck_h01/Mesh1"
    ];
    
    public static void Init()
    {
        IndexMaterials();
    }

    public static void Apply(double currentTime)
    {
        Color nightAmbientLight = AmbientLight.GetAmbientColor(currentTime);
        foreach (Material material in Materials)
        {
            if (material)
            {
                material.SetColor(AmbientLightOverride, nightAmbientLight);
                /*
                if (FormatMaterialName(material.name) == "sph_apollo_interior_ground")
                {
	                // Override for SPH LVL1 concrete pad at entrance
	                float lum = (nightAmbientLight.r + nightAmbientLight.g + nightAmbientLight.b) / 3.0f;
	                if (lum > 1)
	                {
		                lum = 1;
	                }

	                material.color = new(lum, lum, lum);
                }
                */
            }
        }
    }

    private static void IndexMaterials(bool cache = true)
    {
        Materials.Clear();
        
        List<MeshRenderer> meshes;
        if (cache)
        {
		    meshes = Editor.RootExteriorObject?.transform.GetAllChildComponents<MeshRenderer>() ?? [];
        }
        else
        {
            meshes = GameObject.Find("localSpace/Kerbin/Kerbin/KSC/SpaceCenter")?.transform.GetAllChildComponents<MeshRenderer>() ?? [];
        }

        foreach (string meshName in InsideMeshNames)
        {
	        GameObject obj = GameObject.Find(meshName);
	        if (!obj)
	        {
		        continue;
	        }
	        
	        MeshRenderer mr = obj.GetComponent<MeshRenderer>();
	        if (!mr)
	        {
		        continue;
	        }

	        foreach (Material mat in mr.materials)
	        {
		        if (mat)
		        {
			        if (MaterialInfo.TryGetValue(FormatMaterialName(mat.name), out ShaderInfo shaderInfo))
			        {
				        int originalRenderQueue = mat.renderQueue;
				        mat.shader = cache ? shaderInfo.NightShader : shaderInfo.DayShader;
				        mr.gameObject.layer = 15; // Move outside truck back to scenery layer (Deferred Mod side effect)
				        mat.renderQueue = originalRenderQueue;
				        if (cache)
				        {
					        Materials.Add(mat);
				        }
			        }
		        }
	        }
        }

        foreach (MeshRenderer mr in meshes)
        {
	        if (!mr)
	        {
		        continue;
	        }

	        foreach (Material mat in mr.materials)
	        {
		        if (!mat)
		        {
			        continue;
		        }

		        if (MaterialInfo.TryGetValue(FormatMaterialName(mat.name), out ShaderInfo shaderInfo))
		        {
			        int originalRenderQueue = mat.renderQueue;
			        mat.shader = cache ? shaderInfo.NightShader : shaderInfo.DayShader;
			        mat.renderQueue = originalRenderQueue;
			        if (cache)
			        {
				        Materials.Add(mat);
			        }
		        }
	        }

	        foreach (KeyValuePair<string, ShaderInfo> skinnedMeshInfo in MaterialInfoSkinnedMesh)
	        {
		        GameObject obj = GameObject.Find(skinnedMeshInfo.Key);
		        if (obj)
		        {
			        SkinnedMeshRenderer smr = obj.GetComponent<SkinnedMeshRenderer>();
			        if (smr)
			        {
				        Material mat = smr.material;
				        if (mat)
				        {
					        mat.shader = cache ? skinnedMeshInfo.Value.NightShader : skinnedMeshInfo.Value.DayShader;
					        if (cache)
					        {
						        Materials.Add(mat);
					        }
				        }
			        }
		        }
			}
        }
    }

    private static string FormatMaterialName(string materialName)
    {
        return materialName
            .Replace("(Instance)", "")
            .Replace("_UltraQualityGrass", "")
            .Replace("_ultraQualityGrass", "")
            .Replace("_UltraQuality", "")
            .Replace("_ultraQuality", "")
            .Replace("_highUltraQuality", "")
            .Replace("_HighQuality", "")
            .Replace("_highQuality", "")
            .Replace("_lowQuality", "")
            .Trim();
    }
    
    // Since Editor shader changes persist across loads, revert those changes on Flight and SpaceCenter Views
    public static void Reset()
    {
        IndexMaterials(false);
    }
}