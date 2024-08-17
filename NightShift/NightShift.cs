using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KSP.UI.Screens;
using Debug = UnityEngine.Debug;

namespace NightShift;

[KSPAddon(KSPAddon.Startup.EditorAny, false)]
public class NightShift : MonoBehaviour
{
    private readonly List<IDayNightManager> _managers = [];

    // Launcher Variables
    private ApplicationLauncherButton _launcher;

    private bool IsDayMode { get; set; }

    private const ApplicationLauncher.AppScenes VisibleInScenes = ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH;
    
    private readonly Texture2D _iconDay = GameDatabase.Instance.GetTexture("NightShift/Icons/Day", false);
    private readonly Texture2D _iconNight = GameDatabase.Instance.GetTexture("NightShift/Icons/Night", false);


    public void Start()
    {
        IsDayMode = Tools.IsDaytime;
        
        _managers.Add(new LightManager());
        _managers.Add(new SkyboxManager());
        _managers.Add(new ObjectManager());
        _managers.Add(new TextureManager());
        
        // Add the launcher
        GameEvents.onGUIApplicationLauncherReady.Add(AddLauncher);
        GameEvents.onGUIApplicationLauncherDestroyed.Add(RemoveLauncher);
        
        // Callback for editor switching
        GameEvents.onEditorRestart.Add(Switched);

        // Wait before applying settings to prevent dark icon bug
        StartCoroutine(WaitAndUpdateLighting(0.05f));
    }

    public void Update()
    {
        if (!Input.GetKey(KeyCode.LeftAlt)) 
            return;
        
        if (Input.GetKeyDown(KeyCode.N))
        {
            ToggleDayNight();
        }
    }

    public void LateUpdate()
    {
        foreach (var m in _managers)
        {
            if (m is SkyboxManager skyboxManager)
            {
                skyboxManager.Update(IsDayMode);
            }
        }
    }

    public void OnDestroy()
    {
        GameEvents.onGUIApplicationLauncherReady.Remove(AddLauncher);
        GameEvents.onGUIApplicationLauncherDestroyed.Remove(RemoveLauncher);
        GameEvents.onEditorRestart.Remove(Switched);
        RemoveLauncher();
    }

    private void Switched()
    {
        //Debug.Log("[NightShift] Switched between VAB/SPH");

        // Wait a tiny bit before reapplying lighting
        foreach (var mgmt in _managers)
        {
            mgmt.Init();
        }
        
        StartCoroutine(WaitAndUpdateLighting(0.05f));
    }

    private void Refresh()
    {
        foreach (IDayNightManager manager in _managers)
        {
            if (manager is LightManager lm)
            {
                lm.Init();
            }
            
            manager.Switch(IsDayMode);
        }
        
        // Update Launcher Icon
        _launcher?.SetTexture(IsDayMode 
            ? _iconDay 
            : _iconNight);
    }

    private IEnumerator WaitAndUpdateLighting(float timeToWait = 0.0f)
    {
        yield return new WaitForSeconds(timeToWait);
        
        Refresh();
    }

    private void AddLauncher()
    {
        if (ApplicationLauncher.Ready && _launcher == null)
        {
            _launcher = ApplicationLauncher.Instance.AddModApplication(
                ToggleDayNight, null,
                null, null,
                null, null,
                VisibleInScenes, _iconDay
            );
        }

    }

    private void RemoveLauncher()
    {
        if (!_launcher) 
            return;
        
        ApplicationLauncher.Instance.RemoveModApplication(_launcher);
        _launcher = null;
    }

    private void ToggleDayNight()
    {
        IsDayMode = !IsDayMode;
        Refresh();

        // Un-Click Icon
        _launcher?.SetFalse();
    }
}