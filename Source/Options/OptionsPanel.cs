using ICities;
namespace ClassicLightFX.Options
{
    internal static class OptionsPanel
    {
        internal static void Build(UIHelperBase helper)
        {
            var group = helper.AddGroup("ClassicLightFX");
            group.AddButton("VANILLA", FxModule.Release);
            group.AddButton("OPTIMIZED / Default", FxModule.ApplyOptimized);
            group.AddButton("Open compact panel", () => FxModule.OpenStandalone());
            group.AddCheckbox("Apply saved settings when a city loads", ModOptions.Instance.ApplyOnLoad, value => { ModOptions.Instance.ApplyOnLoad = value; ModOptions.Save(true); });
        }
    }
}
