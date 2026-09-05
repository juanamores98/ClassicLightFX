using ColossalFramework.UI;
using ICities;
using ClassicLightFX.Core;

namespace ClassicLightFX.Options
{
    /// <summary>
    /// v2 options panel: lighting, fog and general behavior groups with the
    /// quick presets of this version.
    /// </summary>
    internal static class OptionsPanel
    {
        internal static void Build(UIHelperBase helper)
        {
            ModOptions.Load();
            var options = ModOptions.Instance;

            var lighting = helper.AddGroup("Classic lighting");

            lighting.AddCheckbox("Stock LUT swap", options.SwapLuts, sel =>
            {
                options.SwapLuts = sel;
                ModOptions.Save();
                ClassicTweaks.ReplaceLuts(sel);
            });

            lighting.AddCheckbox("Classic sun color", options.SunColor, sel =>
            {
                options.SunColor = sel;
                ModOptions.Save();
                ClassicTweaks.ReplaceSunlightColor(sel);
            });

            lighting.AddCheckbox("Classic sun strength", options.SunStrength, sel =>
            {
                options.SunStrength = sel;
                ModOptions.Save();
                ClassicTweaks.ReplaceSunlightIntensity(sel);
            });

            lighting.AddCheckbox("Classic sun position", options.SunCoords, sel =>
            {
                options.SunCoords = sel;
                ModOptions.Save();
                ClassicTweaks.ReplaceLatLong(sel);
            });

            var fog = helper.AddGroup("Classic fog");

            fog.AddCheckbox("Classic fog mode", options.ClassicFogMode, sel =>
            {
                options.ClassicFogMode = sel;
                ModOptions.Save();
                ClassicTweaks.ReplaceFogEffect(sel);
            });

            fog.AddCheckbox("Classic fog tint", options.ClassicFogTint, sel =>
            {
                options.ClassicFogTint = sel;
                ModOptions.Save();
            });

            var general = helper.AddGroup("General");

            general.AddCheckbox("Apply when a map loads", options.ApplyOnLoad, sel =>
            {
                options.ApplyOnLoad = sel;
                ModOptions.Save();
            });

            general.AddButton("All classic", () => SetAll(true));
            general.AddButton("All modern", () => SetAll(false));
        }

        private static void SetAll(bool classic)
        {
            var options = ModOptions.Instance;
            options.SwapLuts = classic;
            options.SunColor = classic;
            options.SunStrength = classic;
            options.SunCoords = classic;
            options.ClassicFogMode = classic;
            options.ClassicFogTint = classic;
            ModOptions.Save();

            ClassicTweaks.ReplaceLuts(classic);
            ClassicTweaks.ReplaceSunlightColor(classic);
            ClassicTweaks.ReplaceSunlightIntensity(classic);
            ClassicTweaks.ReplaceLatLong(classic);
            ClassicTweaks.ReplaceFogEffect(classic);
        }
    }
}
