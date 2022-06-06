using System;
using System.Collections;
using UnityEngine;
using KSP.UI.Screens;

namespace NightShift
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class NightShift : MonoBehaviour
    {
        private bool isDaytime = true;

        private SkyboxManager skyboxManager;
        private LightManager lightManager;

        // Launcher Variables
        private static ApplicationLauncherButton launcher;

        private readonly ApplicationLauncher.AppScenes VisibleInScenes =
            ApplicationLauncher.AppScenes.VAB |
            ApplicationLauncher.AppScenes.SPH;

        private readonly Texture2D IconDay = GameDatabase.Instance.GetTexture("NightShift/Textures/Day", false);
        private readonly Texture2D IconNight = GameDatabase.Instance.GetTexture("NightShift/Textures/Night", false);

        public void Start()
        {
            // Make sure we can find the sun, for time calculation at the Space Center
            StartCoroutine(TimeManager.GetSun());

            skyboxManager = new SkyboxManager();
            lightManager = new LightManager();

            isDaytime = TimeManager.IsDaytime();

            // Add the launcher
            GameEvents.onGUIApplicationLauncherReady.Add(AddLauncher);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(RemoveLauncher);

            // Callback for editor switching
            GameEvents.onEditorRestart.Add(Switched);
            //EditorLogic.fetch.switchEditorBtn.onClick.AddListener(Switched);

            // Wait before applying settings to prevent dark icon bug
            StartCoroutine(WaitAndUpdateLighting(0.05f));
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.LeftAlt)) {
                if (Input.GetKeyDown(KeyCode.N))
                {
                    ToggleDayNight();
                }
            }
        }

        public void LateUpdate()
        {
            if (skyboxManager != null && !isDaytime)
            {
                skyboxManager.RotateSkybox();
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
            Debug.Log("[NightShift] Switched between VAB/SPH");

            // Wait a tiny bit before reapplying lighting
            StartCoroutine(WaitAndUpdateLighting(0.05f));

            //GameEvents.onEditorRestart.Add(Switched);
        }

        private void Refresh()
        {
            string dayNight = isDaytime ? "Day" : "Night";
            Debug.Log($"[NightShift] Enabling {dayNight} Settings");

            skyboxManager.EnableDayMode(isDaytime);
            lightManager.EnableDayMode(isDaytime);

            UpdateIcon();
        }

        private IEnumerator WaitAndUpdateLighting(float timeToWait = 0.0f)
        {
            yield return new WaitForSeconds(timeToWait);
            Refresh();
        }

        public void UpdateIcon()
        {
            if (launcher != null)
            {
                if (isDaytime)
                {
                    launcher.SetTexture(IconDay);
                }
                else
                {
                    launcher.SetTexture(IconNight);
                }
            }
        }

        private void AddLauncher()
        {
            if (ApplicationLauncher.Ready && launcher == null)
            {
                launcher = ApplicationLauncher.Instance.AddModApplication(
                    ToggleDayNight, null,
                    null, null,
                    null, null,
                    VisibleInScenes, IconDay
                );
            }

        }

        private void RemoveLauncher()
        {
            if (launcher != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(launcher);
                launcher = null;
            }
        }

        private void ToggleDayNight()
        {
            isDaytime = !isDaytime;
            Refresh();
            UpdateIcon();

            // Un-Click Icon
            launcher.SetFalse();
        }
    }
}
