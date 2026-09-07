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
                ClassicLook.Apply(ClassicFeature.StockTables, sel);
            });

            lighting.AddCheckbox("Classic sun color", options.SunColor, sel =>
            {
                options.SunColor = sel;
                ModOptions.Save();
                ClassicLook.Apply(ClassicFeature.SunGradient, sel);
            });

            lighting.AddCheckbox("Classic sun strength", options.SunStrength, sel =>
            {
                options.SunStrength = sel;
                ModOptions.Save();
                ClassicLook.Apply(ClassicFeature.SunPower, sel);
            });

            lighting.AddCheckbox("Classic sun position", options.SunCoords, sel =>
            {
                options.SunCoords = sel;
                ModOptions.Save();
                ClassicLook.Apply(ClassicFeature.SunPosition, sel);
            });

            var fog = helper.AddGroup("Classic fog");

            fog.AddCheckbox("Classic fog mode", options.ClassicFogMode, sel =>
            {
                options.ClassicFogMode = sel;
                ModOptions.Save();
                ClassicLook.Apply(ClassicFeature.FogEffect, sel);
            });

            fog.AddCheckbox("Classic fog tint", options.ClassicFogTint, sel =>
            {
                options.ClassicFogTint = sel;
                ModOptions.Save();

                // Era el unico interruptor que cambiaba la opcion sin pedir que se aplicara:
                // habia que tocar otra cosa para que surtiera efecto.
                ClassicLook.Apply(ClassicFeature.FogTint, sel);
            });

            var quick = helper.AddGroup("One click");

            quick.AddButton("Vanilla (leave the game untouched)", () => QuickPresets.ApplyVanilla());
            quick.AddButton("Optimized (the calibrated recipe)", () => QuickPresets.ApplyOptimized());

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

            ClassicLook.ApplyFromOptions();
        }
    }
}
