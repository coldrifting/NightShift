using System.Collections;
using UnityEngine;
using KSP.UI.Screens;
using NightShift.Data;
using NightShift.Managers;
using NightShift.Utils;
using Time = NightShift.Utils.Time;

namespace NightShift;

[KSPAddon(KSPAddon.Startup.EditorAny, false)]
public class NightShift : MonoBehaviour
{
    private ApplicationLauncherButton _launcher;

    private const ControlTypes LockMask = ControlTypes.EDITOR_GIZMO_TOOLS | ControlTypes.KEYBOARDINPUT;
    private static KeyBinding _cycleTimeOfDay;

    private static double _visualTime = 0.5f;

    public IEnumerator Start()
    {
        _cycleTimeOfDay = new(KeyCode.N, LockMask);
        
        // Add the launcher
        GameEvents.onGUIApplicationLauncherReady.Add(AddLauncher);
        GameEvents.onGUIApplicationLauncherDestroyed.Add(RemoveLauncher);
        
        // Callback for editor switching
        GameEvents.onEditorRestart.Add(OnStartup);
        
        _visualTime = Time.GetTime();

        yield return new WaitForSeconds(0.05f);
        
        Init();
        Refresh();
    }

    public void Init()
    {
        SkyboxManager.Init(_visualTime);
        
        AmbientManager.Init();
        LightManager.Init();
        ObjectManager.Init();
        WindowManager.Init();
        UvInfo.Init();
    }
    
    public void Apply(double timeOfDay)
    {
        SkyboxManager.Apply(timeOfDay);
        AmbientManager.Apply(timeOfDay);
        LightManager.Apply(timeOfDay);
        ObjectManager.Apply(timeOfDay);
        WindowManager.Apply(timeOfDay);
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (_cycleTimeOfDay.GetKeyDown())
            {
                CycleTimeOfDay();
            }
        }
    }

    public void OnDestroy()
    {
        GameEvents.onGUIApplicationLauncherReady.Remove(AddLauncher);
        GameEvents.onGUIApplicationLauncherDestroyed.Remove(RemoveLauncher);
        GameEvents.onEditorRestart.Remove(OnStartup);
        RemoveLauncher();
    }

    private void OnStartup()
    {
        Init();
        
        StartCoroutine(WaitThenRefresh());
    }
    
    private IEnumerator WaitThenRefresh(float timeToWait = 0.1f)
    {
        yield return new WaitForSeconds(timeToWait);

        Refresh();
    }

    private void Refresh()
    {
        Apply(_visualTime);
        
        // Update Launcher Icon
        Texture2D icon = GetAppIcon();
        _launcher?.SetTexture(icon);
    }

    private Texture2D GetAppIcon()
    {
        return Time.GetTimeOfDay(_visualTime) switch
        {
            TimeOfDay.Dawn => Constants.AppIconDawn,
            TimeOfDay.Day => Constants.AppIconDay,
            TimeOfDay.Dusk => Constants.AppIconDusk,
            TimeOfDay.Night => Constants.AppIconNight,
            _ => Constants.AppIconDay
        };
    }

    private void AddLauncher()
    {
        if (ApplicationLauncher.Ready && !_launcher)
        {
            Texture2D icon = GetAppIcon();
            _launcher = ApplicationLauncher.Instance.AddModApplication(
                CycleTimeOfDay, null,
                null, null,
                null, null,
                ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH, icon
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

    private void CycleTimeOfDay()
    {
        _visualTime = Time.NextTimeOfDay(_visualTime);
        
        Refresh();

        // Un-Click Icon
        _launcher?.SetFalse();
    }

    public static void Log(string msg)
    {
        Debug.Log("[NightShift] " + msg);
    }

    public static void Warn(string msg)
    {
        Debug.LogWarning("[NightShift] " + msg);
    }
}

// Since shader changes are persisted outside of editor, we need to reset shaders once we leave.
[KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
public class NightShiftCleanup : MonoBehaviour
{
    public void Start()
    {
        AmbientManager.Init();
        AmbientManager.Reset();
    }
}
