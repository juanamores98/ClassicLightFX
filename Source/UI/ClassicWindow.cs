using UnityEngine;
using ClassicLightFX.Core;
using ClassicLightFX.Options;

namespace ClassicLightFX.UI
{
    /// <summary>
    /// In-game quick panel: the classic restoration switches grouped by
    /// concern, with live application and the apply-on-load behavior.
    /// </summary>
    internal sealed class ClassicWindow
    {
        private Rect _rect = new Rect(920f, 140f, 430f, 360f);

        internal void Draw(int id)
        {
            _rect = GUI.Window(id, _rect, DrawWindow, "ClassicLightFX v2");
        }

        private void DrawWindow(int id)
        {
            GUI.DragWindow(new Rect(0f, 0f, 410f, 22f));
            if (GUI.Button(new Rect(_rect.width - 26f, 4f, 22f, 18f), "x"))
            {
                ClassicEngine.CloseWindow();
            }

            var options = ModOptions.Instance;
            float y = 32f;

            y = Section("Lighting", y);
            options.SwapLuts = Switch("Restore classic stock LUTs", options.SwapLuts, y, v => ClassicTweaks.ReplaceLuts(v)); y += 26f;
            options.SunColor = Switch("Restore classic sun color", options.SunColor, y, v => ClassicTweaks.ReplaceSunlightColor(v)); y += 26f;
            options.SunStrength = Switch("Restore classic sun strength", options.SunStrength, y, v => ClassicTweaks.ReplaceSunlightIntensity(v)); y += 26f;
            options.SunCoords = Switch("Restore classic sun position", options.SunCoords, y, v => ClassicTweaks.ReplaceLatLong(v)); y += 28f;

            y = Section("Fog", y);
            options.ClassicFogMode = Switch("Prefer the classic fog effect", options.ClassicFogMode, y, v => ClassicTweaks.ReplaceFogEffect(v)); y += 26f;
            options.ClassicFogTint = Switch("Classic fog tint over modern fog", options.ClassicFogTint, y, null); y += 28f;

            y = Section("Behavior", y);
            options.ApplyOnLoad = GUI.Toggle(new Rect(10f, y, 400f, 24f), options.ApplyOnLoad, " Apply the saved profile when a map loads");
            y += 30f;

            if (GUI.Button(new Rect(10f, y, 200f, 26f), "All classic"))
            {
                SetAll(true);
            }

            if (GUI.Button(new Rect(220f, y, 200f, 26f), "All modern"))
            {
                SetAll(false);
            }

            ModOptions.Save();
        }

        private static float Section(string title, float y)
        {
            GUI.Label(new Rect(10f, y, 300f, 22f), "<b>" + title + "</b>");
            return y + 24f;
        }

        private static bool Switch(string label, bool value, float y, System.Action<bool> apply)
        {
            bool result = GUI.Toggle(new Rect(10f, y, 410f, 24f), value, " " + label);
            if (result != value && apply != null)
            {
                apply(result);
            }

            return result;
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

            ClassicTweaks.ReplaceLuts(classic);
            ClassicTweaks.ReplaceSunlightColor(classic);
            ClassicTweaks.ReplaceSunlightIntensity(classic);
            ClassicTweaks.ReplaceLatLong(classic);
            ClassicTweaks.ReplaceFogEffect(classic);
        }
    }
}
