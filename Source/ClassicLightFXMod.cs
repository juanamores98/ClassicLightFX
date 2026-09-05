using ICities;
using ClassicLightFX.Core;
using ClassicLightFX.Options;

namespace ClassicLightFX
{
    /// <summary>
    /// ClassicLightFX v2 entry point: restores the pre-After Dark look with
    /// its own option model and lifecycle.
    /// </summary>
    public class ClassicLightFXMod : IUserMod
    {
        private const string Version = "2.0.0";

        public string Name
        {
            get { return "ClassicLightFX v2"; }
        }

        public string Description
        {
            get { return "Restores the pre-After Dark lighting: LUTs, sun color, strength and position, classic fog. v" + Version; }
        }

        public void OnEnabled()
        {
            ModOptions.Load();
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            OptionsPanel.Build(helper);
        }
    }

    public class LoadingExtension : LoadingExtensionBase
    {
        private const string HostObjectName = "ClassicLightFX2";

        private UnityEngine.GameObject _host;

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            ModOptions.Load();
            if (ModOptions.Instance.ApplyOnLoad)
            {
                ClassicTweaks.SetUp();
            }

            DestroyHosts();
            _host = new UnityEngine.GameObject(HostObjectName);
            _host.AddComponent<Core.ClassicEngine>();

            UI.UuiButton.Register(
                "ClassicLightFX v2",
                "Classic lighting quick switches (F9)",
                UI.TrayIcon.Make(),
                show => Core.ClassicEngine.OpenWindow());
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            UI.UuiButton.Unregister();
            DestroyHosts();
            ClassicTweaks.CleanUp();
        }

        private static void DestroyHosts()
        {
            while (true)
            {
                var leftover = UnityEngine.GameObject.Find(HostObjectName);
                if (!leftover)
                {
                    break;
                }

                UnityEngine.Object.DestroyImmediate(leftover);
            }
        }
    }
}
