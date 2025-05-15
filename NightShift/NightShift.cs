using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KSP.UI.Screens;
using NightShift.Managers;

namespace NightShift;

[KSPAddon(KSPAddon.Startup.EditorAny, false)]
public class NightShift : MonoBehaviour
{
    private readonly List<IDayNightManager> _managers = [];

    // Launcher Variables
    private ApplicationLauncherButton _launcher;

    private TimeOfDay TimeOfDay { get; set; }

    private const ApplicationLauncher.AppScenes VisibleInScenes = ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH;
    
    private readonly Texture2D _iconDay = GameDatabase.Instance.GetTexture("NightShift/Icons/Day", false);
    private readonly Texture2D _iconTwilight = GameDatabase.Instance.GetTexture("NightShift/Icons/Twilight", false);
    private readonly Texture2D _iconNight = GameDatabase.Instance.GetTexture("NightShift/Icons/Night", false);

    private KeyBinding _cycleTimeOfDay;

    public void Start()
    {
        _cycleTimeOfDay = new(KeyCode.N, ControlTypes.EDITOR_GIZMO_TOOLS | ControlTypes.KEYBOARDINPUT);
        
        _managers.Add(new LightManager());
        _managers.Add(new SkyboxManager());
        _managers.Add(new ObjectManager());
        _managers.Add(new TextureManager());
        
        // Add the launcher
        GameEvents.onGUIApplicationLauncherReady.Add(AddLauncher);
        GameEvents.onGUIApplicationLauncherDestroyed.Add(RemoveLauncher);
        
        // Callback for editor switching
        GameEvents.onEditorRestart.Add(Switched);

        TimeOfDay = Tools.GetTimeOfDay();

        foreach (var mgmt in _managers)
        {
            mgmt.Apply(TimeOfDay);
        }
    }

    public void Update()
    {
        if (_cycleTimeOfDay.GetKeyDown())
        {
            ToggleDayNight();
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
        foreach (var mgmt in _managers)
        {
            mgmt.Init();
            mgmt.Apply(TimeOfDay);
        }
        
        StartCoroutine(WaitAndUpdateLighting(0.05f));
    }
    
    public Texture2D FlipTexture(Texture2D original, bool vertical = true)
    {
        Texture2D flipped = new Texture2D(original.width, original.height);

        int xN = original.width;
        int yN = original.height;


        for (int i = 0; i < xN; i++)
        {
            for (int j = 0; j < yN; j++)
            {
                if (vertical)
                {
                    flipped.SetPixel(j, xN - i - 1, original.GetPixel(j, i));
                }
                else
                {
                    flipped.SetPixel(xN - i - 1, j, original.GetPixel(i, j));
                }
            }
        }
        flipped.Apply();

        return flipped;
    }

    private void Refresh()
    {
        foreach (IDayNightManager manager in _managers)
        {
            manager.Apply(TimeOfDay);
        }
        
        // Update Launcher Icon
        Texture2D icon = GetAppIcon(TimeOfDay);

        _launcher?.SetTexture(icon);
    }

    private Texture2D GetAppIcon(TimeOfDay timeOfDay)
    {
        return timeOfDay switch
        {
            TimeOfDay.Day => _iconDay,
            TimeOfDay.Twilight => _iconTwilight,
            TimeOfDay.Night => _iconNight,
            _ => _iconDay
        };
    }

    private IEnumerator WaitAndUpdateLighting(float timeToWait = 0.0f)
    {
        yield return new WaitForSeconds(timeToWait);
        
        Refresh();
    }

    private void AddLauncher()
    {
        if (ApplicationLauncher.Ready && !_launcher)
        {
            Texture2D icon = GetAppIcon(TimeOfDay);
            _launcher = ApplicationLauncher.Instance.AddModApplication(
                ToggleDayNight, null,
                null, null,
                null, null,
                VisibleInScenes, icon
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
        TimeOfDay = TimeOfDay.Cycle();
        Refresh();

        // Un-Click Icon
        _launcher?.SetFalse();
    }
}