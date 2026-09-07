using UnityEngine;
using ClassicLightFX.Core;
using ClassicLightFX.Options;

namespace ClassicLightFX.UI
{
    /// <summary>
    /// In-game quick panel for ClassicLightFX v2: 3 one-click archetypes
    /// (Pre-After Dark 2015, Híbrido Equilibrado, Moderno) followed by granular
    /// lighting and fog restoration switches with live application.
    /// </summary>
    internal sealed class ClassicWindow
    {
        private Rect _rect;

        internal ClassicWindow()
        {
            var opt = ModOptions.Instance;
            float x = opt.WindowX > 0f ? opt.WindowX : 900f;
            float y = opt.WindowY > 0f ? opt.WindowY : 120f;
            if (Screen.width > 0 && Screen.height > 0)
            {
                x = Mathf.Clamp(x, 10f, Mathf.Max(10f, Screen.width - 450f));
                y = Mathf.Clamp(y, 10f, Mathf.Max(10f, Screen.height - 470f));
            }
            _rect = new Rect(x, y, 440f, 460f);
        }

        internal void Draw(int id)
        {
            float oldX = _rect.x;
            float oldY = _rect.y;
            _rect = GUI.Window(id, _rect, DrawWindow, "ClassicLightFX Studio v2");
            if (!Mathf.Approximately(oldX, _rect.x) || !Mathf.Approximately(oldY, _rect.y))
            {
                ModOptions.Instance.WindowX = _rect.x;
                ModOptions.Instance.WindowY = _rect.y;
                ModOptions.Save(false);
            }
        }

        private void DrawWindow(int id)
        {
            GUI.DragWindow(new Rect(0f, 0f, _rect.width - 30f, 22f));
            if (GUI.Button(new Rect(_rect.width - 26f, 4f, 22f, 18f), "x"))
            {
                ClassicEngine.CloseWindow();
            }

            var options = ModOptions.Instance;
            float y = 28f;

            // -----------------------------------------------------------------
            // 3 Archetypes of 1-Click
            // -----------------------------------------------------------------
            y = Section("ONE-CLICK LOOKS", y);
            if (GUI.Button(new Rect(10f, y, 134f, 26f), "Pre-AD (2015)"))
            {
                SetArchetype(0);
            }
            if (GUI.Button(new Rect(150f, y, 134f, 26f), "Hybrid"))
            {
                SetArchetype(1);
            }
            if (GUI.Button(new Rect(290f, y, 138f, 26f), "Moderno (2026)"))
            {
                SetArchetype(2);
            }
            y += 32f;

            // -----------------------------------------------------------------
            // Granular Lighting Switches
            // -----------------------------------------------------------------
            y = Section("CLASSIC PRE-AFTER DARK LIGHTING", y);
            options.SwapLuts = Switch("Restaurar tablas LUT originales (Stock LUTs)", options.SwapLuts, y, v => ClassicLook.Apply(ClassicFeature.StockTables, v)); y += 26f;
            options.SunColor = Switch("Restaurar color del sol pre-After Dark", options.SunColor, y, v => ClassicLook.Apply(ClassicFeature.SunGradient, v)); y += 26f;
            options.SunStrength = Switch("Restaurar fuerza del sol original", options.SunStrength, y, v => ClassicLook.Apply(ClassicFeature.SunPower, v)); y += 26f;
            options.SunCoords = Switch("Classic sun position", options.SunCoords, y, v => ClassicLook.Apply(ClassicFeature.SunPosition, v)); y += 28f;

            // -----------------------------------------------------------------
            // Granular Fog Switches
            // -----------------------------------------------------------------
            y = Section("NIEBLA ORIGINAL Y COMPATIBILIDAD CON CICLO", y);
            options.ClassicFogMode = Switch("Classic fog shader", options.ClassicFogMode, y, v => ClassicLook.Apply(ClassicFeature.FogEffect, v)); y += 26f;
            options.ClassicFogTint = Switch("Classic fog tint over modern fog", options.ClassicFogTint, y, v => ClassicLook.Apply(ClassicFeature.FogTint, v)); y += 26f;
            options.ClassicFogWithCycle = Switch("Keep classic fog with the day/night cycle on", options.ClassicFogWithCycle, y, v => ClassicLook.Apply(ClassicFeature.FogEffect, v)); y += 28f;

            // -----------------------------------------------------------------
            // Behavior & Suite Presets
            // -----------------------------------------------------------------
            y = Section("PERSISTENCIA Y SUITE", y);
            bool applyOnLoad = GUI.Toggle(new Rect(10f, y, 410f, 22f), options.ApplyOnLoad, " Aplicar el perfil guardado al cargar una partida");
            if (applyOnLoad != options.ApplyOnLoad)
            {
                options.ApplyOnLoad = applyOnLoad;
                ModOptions.Save(false);
            }
            y += 28f;

            if (GUI.Button(new Rect(10f, y, 200f, 26f), "Vanilla"))
            {
                QuickPresets.ApplyVanilla();
            }

            if (GUI.Button(new Rect(220f, y, 208f, 26f), "Optimized"))
            {
                QuickPresets.ApplyOptimized();
            }
        }

        private static float Section(string title, float y)
        {
            GUI.Label(new Rect(10f, y, 420f, 22f), "<b><color=#4FC3F7>" + title + "</color></b>");
            return y + 24f;
        }

        private static bool Switch(string label, bool value, float y, System.Action<bool> apply)
        {
            bool result = GUI.Toggle(new Rect(10f, y, 420f, 24f), value, " " + label);
            if (result != value)
            {
                if (apply != null)
                {
                    apply(result);
                }
                ModOptions.Save(false);
            }

            return result;
        }

        private static void SetArchetype(int mode)
        {
            var options = ModOptions.Instance;
            if (mode == 0) // Pre-After Dark (2015)
            {
                options.SwapLuts = true;
                options.SunColor = true;
                options.SunStrength = true;
                options.SunCoords = true;
                options.ClassicFogMode = true;
                options.ClassicFogTint = true;
                options.ClassicFogWithCycle = false;
            }
            else if (mode == 1) // Híbrido Equilibrado
            {
                options.SwapLuts = true;
                options.SunColor = true;
                options.SunStrength = true;
                options.SunCoords = true;
                options.ClassicFogMode = false;
                options.ClassicFogTint = true;
                options.ClassicFogWithCycle = true;
            }
            else // Moderno (2026)
            {
                options.SwapLuts = false;
                options.SunColor = false;
                options.SunStrength = false;
                options.SunCoords = false;
                options.ClassicFogMode = false;
                options.ClassicFogTint = false;
                options.ClassicFogWithCycle = false;
            }

            ClassicLook.ApplyFromOptions();
            ModOptions.Save(true);
        }
    }
}
