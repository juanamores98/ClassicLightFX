using System;
using System.IO;
using ColossalFramework.UI;
using UnityEngine;
using ClassicLightFX.UI;

namespace ClassicLightFX
{
    /// <summary>Small public boundary for a standalone or embedded native panel.</summary>
    public static class FxModule
    {
        public const float PreferredWidth = 380f;
        public const float PreferredHeight = 540f;
        private static PanelView _standalone;
        public static string Mode { get { return Options.ModOptions.Instance.VanillaMode ? "GAME" : (Infrastructure.FxStorage.MatchesOptimized(ReadState(), typeof(ClassicLightFXMod)) ? "DEFAULT v3" : "CUSTOM"); } }
        public static string ReadState() { return ClassicLightFXMod.ExportSuiteSection(); }
        public static bool ApplyState(string xml) { return ClassicLightFXMod.ApplySuiteSection(xml); }
        public static void Release() { if (!Core.QuickPresets.ApplyVanilla()) throw new InvalidOperationException("VANILLA could not be applied."); Flush(); }
        public static void ApplyOptimized() { if (!Core.QuickPresets.ApplyOptimized()) throw new InvalidOperationException(ClassicLightFXMod.LastApplyError ?? "Default could not be applied."); Flush(); }
        public static void Flush() { Options.ModOptions.SaveImmediate(); }
        public static string Status { get { return !string.IsNullOrEmpty(Infrastructure.FxStorage.LastError) ? Infrastructure.FxStorage.LastError : !string.IsNullOrEmpty(Infrastructure.PropertyLedger.LastWarning) ? Infrastructure.PropertyLedger.LastWarning : Mode + (Infrastructure.FxInterop.Claims("LumenFX.LumenFXMod", "lightColor") ? " · Light controlled by LumenFX" : "") + (Infrastructure.FxInterop.Claims("AtmosphereFX.AtmosphereFXMod", "fogEffect") ? " · Fog controlled by AtmosphereFX" : ""); } }

        public static PanelView CreatePanel(UIComponent parent, float width = PreferredWidth, float height = PreferredHeight)
        {
            var view = new PanelView("ClassicLightFX", parent, width, height, Release, ApplyOptimized, () => Status);
            var page0 = view.AddPage("Light");
            view.Action(page0, "🏛️ Pre-After Dark (2015)", () => Edit(() => {
                var o = Options.ModOptions.Instance;
                o.SwapLuts = o.SunColor = o.SunStrength = o.SunCoords = o.ClassicFogMode = o.ClassicFogTint = true;
                o.ClassicFogWithCycle = false;
            }));
            view.Action(page0, "🌓 Hybrid", () => Edit(() => {
                var o = Options.ModOptions.Instance;
                o.SwapLuts = o.SunColor = o.SunStrength = true;
                o.SunCoords = false;
                o.ClassicFogMode = o.ClassicFogTint = true;
                o.ClassicFogWithCycle = true;
            }));
            view.Action(page0, "🏙️ Modern (Vanilla)", () => Release());
            view.Check(page0, "Procedural classic stock LUTs", () => Options.ModOptions.Instance.SwapLuts, v => Edit(() => Options.ModOptions.Instance.SwapLuts = v));
            view.Check(page0, "Classic daylight approximation", () => Options.ModOptions.Instance.SunColor, v => Edit(() => Options.ModOptions.Instance.SunColor = v));
            view.Check(page0, "Classic sun strength", () => Options.ModOptions.Instance.SunStrength, v => Edit(() => Options.ModOptions.Instance.SunStrength = v));
            view.Check(page0, "Classic sun coordinates", () => Options.ModOptions.Instance.SunCoords, v => Edit(() => Options.ModOptions.Instance.SunCoords = v));
            view.Info(page0, () => "Sun: " + (!Options.ModOptions.Instance.SunColor && !Options.ModOptions.Instance.SunStrength ? "off" : Infrastructure.FxInterop.Claims("LumenFX.LumenFXMod", "lightColor") ? "applied through LumenFX" : "standalone"));
            view.Info(page0, () => Options.ModOptions.Instance.SunCoords && Infrastructure.FxInterop.Claims("SceneFX.SceneFXMod", "sunPosition")
                ? "Classic coordinates applied through SceneFX; its saved position is preserved" : "Coordinates follow the selected classic option");
            view.Info(page0, () => Options.ModOptions.Instance.SwapLuts && Infrastructure.FxInterop.Claims("SceneFX.SceneFXMod", "lut")
                ? "Classic stock LUT replacement blocked while SceneFX owns LUT selection" : "Classic LUTs are procedural approximations");
            var page1 = view.AddPage("Fog");
            view.Check(page1, "Classic fog effect", () => Options.ModOptions.Instance.ClassicFogMode, v => Edit(() => Options.ModOptions.Instance.ClassicFogMode = v));
            view.Check(page1, "Classic atmospheric tint", () => Options.ModOptions.Instance.ClassicFogTint, v => Edit(() => Options.ModOptions.Instance.ClassicFogTint = v));
            view.Check(page1, "Allow classic fog with day/night cycle", () => Options.ModOptions.Instance.ClassicFogWithCycle, v => Edit(() => Options.ModOptions.Instance.ClassicFogWithCycle = v));
            view.Check(page1, "Apply settings when a city loads", () => Options.ModOptions.Instance.ApplyOnLoad, v => { Options.ModOptions.Instance.ApplyOnLoad = v; Options.ModOptions.Save(); });
            view.Info(page1, () => "Fog: " + (!Options.ModOptions.Instance.ClassicFogMode ? "off" : Infrastructure.FxInterop.Claims("AtmosphereFX.AtmosphereFXMod", "fogEffect") ? "applied through AtmosphereFX" : "standalone"));
            view.Info(page1, () => ClassicLightFXMod.ApplicationStatus ?? "Settings ready; appearance not yet verified in game");
            view.Refresh();
            return view;
        }

        internal static void OpenStandalone(bool toggle = false)
        {
            if (_standalone == null || _standalone.Root == null)
            {
                _standalone = CreatePanel(null, PreferredWidth, Mathf.Min(PreferredHeight, UIView.GetAView().fixedHeight - 24f));
                _standalone.Root.relativePosition = new Vector3(Mathf.Clamp(WindowX, 0f, Mathf.Max(0f, UIView.GetAView().fixedWidth - PreferredWidth)), Mathf.Clamp(WindowY, 0f, Mathf.Max(0f, UIView.GetAView().fixedHeight - _standalone.Root.height)));
                _standalone.Root.eventPositionChanged += (c, value) => { WindowX = value.x; WindowY = value.y; SavePosition(); };
            }
            else _standalone.Root.isVisible = toggle ? !_standalone.Root.isVisible : true;
            var screen = UIView.GetAView();
            _standalone.SetSize(PreferredWidth, Mathf.Min(PreferredHeight, screen.fixedHeight - 24f));
            var pos = _standalone.Root.relativePosition;
            _standalone.Root.relativePosition = new Vector3(Mathf.Clamp(pos.x, 0f, Mathf.Max(0f, screen.fixedWidth - _standalone.Root.width)), Mathf.Clamp(pos.y, 0f, Mathf.Max(0f, screen.fixedHeight - _standalone.Root.height)));
            _standalone.Refresh();
        }

        internal static void CloseStandalone()
        {
            if (_standalone != null) _standalone.Dispose();
            _standalone = null;
        }
        private static void Edit(Action edit)
        {
            edit(); Options.ModOptions.Instance.VanillaMode = false;
            Core.ClassicLook.ApplyFromOptions(); Options.ModOptions.Save(); ClassicLightFXMod.NotifyStateChanged();
        }
        private static float WindowX { get { return Options.ModOptions.Instance.WindowX; } set { Options.ModOptions.Instance.WindowX = value; } }
        private static float WindowY { get { return Options.ModOptions.Instance.WindowY; } set { Options.ModOptions.Instance.WindowY = value; } }
        private static void SavePosition() { Options.ModOptions.Save(); }
    }
}
