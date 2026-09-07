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
        /// <summary>
        /// Alto que ocupo el contenido la ultima vez que se dibujo.
        /// </summary>
        /// <remarks>
        /// Las ventanas tenian un alto fijo escrito a mano. Al crecer el contenido, lo ultimo
        /// —que suele ser un boton— quedaba fuera del recorte y no habia forma de pulsarlo.
        /// Medirlo mientras se dibuja y ajustar la ventana en el siguiente fotograma cuesta un
        /// fotograma de retraso y ningun numero que mantener a mano.
        /// </remarks>
        private float _contentHeight;

        private Rect _rect;

        internal ClassicWindow()
        {
            var opt = ModOptions.Instance;
            float x = opt.WindowX > 0f ? opt.WindowX : 920f;
            float y = opt.WindowY > 0f ? opt.WindowY : 140f;
            if (Screen.width > 0 && Screen.height > 0)
            {
                x = Mathf.Clamp(x, 10f, Mathf.Max(10f, Screen.width - 440f));
                y = Mathf.Clamp(y, 10f, Mathf.Max(10f, Screen.height - 380f));
            }
            _rect = new Rect(x, y, 430f, 360f);
        }

        internal void Draw(int id)
        {
            float oldX = _rect.x;
            float oldY = _rect.y;
            // Que quepa lo que haya, sin salirse de la pantalla.
            if (_contentHeight > 0f)
            {
                _rect.height = Mathf.Min(_contentHeight + 12f, Screen.height - 60f);
            }

            _rect = GUI.Window(id, _rect, DrawWindow, "ClassicLightFX v2");
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
            float y = 32f;

            y = Section("LIGHTING RESTORATION", y);
            options.SwapLuts = Switch("Restore classic stock LUTs", options.SwapLuts, y, v => ClassicLook.Apply(ClassicFeature.StockTables, v)); y += 26f;
            options.SunColor = Switch("Restore classic sun color", options.SunColor, y, v => ClassicLook.Apply(ClassicFeature.SunGradient, v)); y += 26f;
            options.SunStrength = Switch("Restore classic sun strength", options.SunStrength, y, v => ClassicLook.Apply(ClassicFeature.SunPower, v)); y += 26f;
            options.SunCoords = Switch("Restore classic sun position", options.SunCoords, y, v => ClassicLook.Apply(ClassicFeature.SunPosition, v)); y += 28f;

            y = Section("FOG MODES", y);
            options.ClassicFogMode = Switch("Prefer the classic fog effect", options.ClassicFogMode, y, v => ClassicLook.Apply(ClassicFeature.FogEffect, v)); y += 26f;
            options.ClassicFogTint = Switch("Classic fog tint over modern fog", options.ClassicFogTint, y,
                v => ClassicLook.Apply(ClassicFeature.FogTint, v)); y += 28f;

            y = Section("BEHAVIOR", y);
            bool applyOnLoad = GUI.Toggle(new Rect(10f, y, 400f, 24f), options.ApplyOnLoad, " Apply the saved profile when a map loads");
            if (applyOnLoad != options.ApplyOnLoad)
            {
                options.ApplyOnLoad = applyOnLoad;
                ModOptions.Save(false);
            }
            y += 30f;

            if (GUI.Button(new Rect(10f, y, 200f, 26f), "All classic"))
            {
                SetAll(true);
            }

            if (GUI.Button(new Rect(220f, y, 200f, 26f), "All modern"))
            {
                SetAll(false);
            }

            y += 32f;

            if (GUI.Button(new Rect(10f, y, 200f, 26f), "Vanilla"))
            {
                QuickPresets.ApplyVanilla();
            }

            if (GUI.Button(new Rect(220f, y, 200f, 26f), "Optimized"))
            {
                QuickPresets.ApplyOptimized();
            }

            _contentHeight = y + 32f;
        }

        private static float Section(string title, float y)
        {
            GUI.Label(new Rect(10f, y, 350f, 22f), "<b><color=#4FC3F7>" + title + "</color></b>");
            return y + 24f;
        }

        private static bool Switch(string label, bool value, float y, System.Action<bool> apply)
        {
            bool result = GUI.Toggle(new Rect(10f, y, 410f, 24f), value, " " + label);
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

        private static void SetAll(bool classic)
        {
            var options = ModOptions.Instance;
            options.SwapLuts = classic;
            options.SunColor = classic;
            options.SunStrength = classic;
            options.SunCoords = classic;
            options.ClassicFogMode = classic;
            options.ClassicFogTint = classic;

            ClassicLook.ApplyFromOptions();
            ModOptions.Save(true);
        }
    }
}

