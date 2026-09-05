using UnityEngine;
using ClassicLightFX.Core;
using ClassicLightFX.Options;

namespace ClassicLightFX.UI
{
    /// <summary>
    /// In-game quick panel: the six classic switches with live application.
    /// </summary>
    internal sealed class ClassicWindow
    {
        private Rect _rect = new Rect(240f, 180f, 420f, 300f);

        internal void Draw(int id)
        {
            _rect = GUI.Window(id, _rect, DrawWindow, "ClassicLightFX v2");
        }

        private void DrawWindow(int id)
        {
            GUI.DragWindow(new Rect(0f, 0f, 400f, 22f));
            if (GUI.Button(new Rect(_rect.width - 26f, 4f, 22f, 18f), "x"))
            {
                ClassicEngine.CloseWindow();
            }

            var options = ModOptions.Instance;
            float y = 34f;

            options.SwapLuts = Switch("Stock LUT swap", options.SwapLuts, y, v => ClassicTweaks.ReplaceLuts(v)); y += 28f;
            options.SunColor = Switch("Classic sun color", options.SunColor, y, v => ClassicTweaks.ReplaceSunlightColor(v)); y += 28f;
            options.SunStrength = Switch("Classic sun strength", options.SunStrength, y, v => ClassicTweaks.ReplaceSunlightIntensity(v)); y += 28f;
            options.SunCoords = Switch("Classic sun position", options.SunCoords, y, v => ClassicTweaks.ReplaceLatLong(v)); y += 28f;
            options.ClassicFogMode = Switch("Classic fog mode", options.ClassicFogMode, y, v => ClassicTweaks.ReplaceFogEffect(v)); y += 28f;
            options.ClassicFogTint = Switch("Classic fog tint", options.ClassicFogTint, y, null); y += 32f;

            if (GUI.Button(new Rect(10f, y, 195f, 26f), "All classic"))
            {
                SetAll(true);
            }

            if (GUI.Button(new Rect(215f, y, 195f, 26f), "All modern"))
            {
                SetAll(false);
            }

            ModOptions.Save();
        }

        private static bool Switch(string label, bool value, float y, System.Action<bool> apply)
        {
            bool result = GUI.Toggle(new Rect(10f, y, 390f, 24f), value, " " + label);
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
