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
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            ModOptions.Load();
            if (ModOptions.Instance.ApplyOnLoad)
            {
                ClassicTweaks.SetUp();
            }
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            ClassicTweaks.CleanUp();
        }
    }
}
