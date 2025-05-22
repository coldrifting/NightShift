using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KSP.UI.Screens;
using NightShift.Data;
using NightShift.Managers;
using NightShift.Managers.Interface;
using NightShift.Utils;

namespace NightShift;

[KSPAddon(KSPAddon.Startup.EditorAny, false)]
public class NightShift : MonoBehaviour
{
    private ApplicationLauncherButton _launcher;
    private readonly List<IDayNightManager> _managers = [];
    private EditorTimeDisplay _editorTimeDisplay;

     // TODO - Poll periodically for changes
    private double _currentTime = 0.5;
    private double CurrentTime
    {
        get => _currentTime;
        set => _currentTime = (value + 1) % 1.0;
    }

    private const ControlTypes LockMask = ControlTypes.EDITOR_GIZMO_TOOLS | ControlTypes.KEYBOARDINPUT;
    private KeyBinding _cycleTimeOfDay;
    
    private KeyBinding _setVisualTimeBack;
    private KeyBinding _setVisualTimeForward;
    private KeyBinding _setVisualTimeStop;

    private TimeMode _timeMode = TimeMode.Pause;
    private double _timeScale = 0.0625;

    private Coroutine _timeAdvance = null;

    public IEnumerator Start()
    {
        _cycleTimeOfDay = new(KeyCode.N, LockMask);

        _setVisualTimeBack = new(KeyCode.Comma, LockMask);
        _setVisualTimeForward = new(KeyCode.Period, LockMask);
        _setVisualTimeStop = new(KeyCode.Slash, LockMask);
        
        _managers.Add(new SkyboxManager());
        _managers.Add(new AmbientManager());
        _managers.Add(new LightManager());
        _managers.Add(new ObjectManager());
        _managers.Add(new WindowManager());
        
        // Add the launcher
        GameEvents.onGUIApplicationLauncherReady.Add(AddLauncher);
        GameEvents.onGUIApplicationLauncherDestroyed.Add(RemoveLauncher);
        
        // Callback for editor switching
        GameEvents.onEditorRestart.Add(OnStartup);

        _editorTimeDisplay = new();
        
        CurrentTime = Time.GetTime();

        yield return new WaitForSeconds(0.1f);
        
        Init();
        Refresh();
                    
        _timeAdvance = StartCoroutine(AdvanceTime());
    }

    public void Init()
    {
        foreach (var manager in _managers)
        {
            manager.Init();
        }
    }
    
    public void Apply(double timeOfDay)
    {
        foreach (var manager in _managers)
        {
            manager.Apply(timeOfDay);
        }
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (_cycleTimeOfDay.GetKeyDown())
            {
                CycleTimeOfDay();
            }
            else if (_setVisualTimeStop.GetKeyDown())
            {
                _timeMode = TimeMode.Pause;
            }
            else if (_setVisualTimeBack.GetKeyDown())
            {
                _timeMode = _timeMode.Prev();
            }
            else if (_setVisualTimeForward.GetKeyDown())
            {
                _timeMode = _timeMode.Next();
            }
        }
    }

    private IEnumerator AdvanceTime()
    {
        while (true)
        {  
            CurrentTime = _timeMode switch
            {
                TimeMode.ReReWind => CurrentTime - UnityEngine.Time.deltaTime * _timeScale * 4,
                TimeMode.Rewind => CurrentTime - UnityEngine.Time.deltaTime * _timeScale * 2,
                TimeMode.Back => CurrentTime - UnityEngine.Time.deltaTime * _timeScale,
                TimeMode.Forward => CurrentTime + UnityEngine.Time.deltaTime * _timeScale,
                TimeMode.FastForward => CurrentTime + UnityEngine.Time.deltaTime * _timeScale * 2,
                TimeMode.FastFastForward => CurrentTime + UnityEngine.Time.deltaTime * _timeScale * 4,
                _ => CurrentTime
            };
            
            _editorTimeDisplay.Update(_timeMode, CurrentTime);
            if (_timeMode != TimeMode.Pause)
            {
                Refresh();
            }
            
            yield return new WaitForEndOfFrame();
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
        Apply(CurrentTime);
        
        // Update Launcher Icon
        Texture2D icon = GetAppIcon();
        _launcher?.SetTexture(icon);
    }

    private Texture2D GetAppIcon()
    {
        return Time.GetTimeOfDay(CurrentTime) switch
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
        CurrentTime = Time.NextTimeOfDay(CurrentTime);
        
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
        AmbientManager shaderAmbientManager = new();
        shaderAmbientManager.Reset();
    }
}
