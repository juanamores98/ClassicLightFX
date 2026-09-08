using System.Collections.Generic;
using ClassicLightFX.Infrastructure;
using UnityEngine;
using ColossalFramework;
using ClassicLightFX.Options;

namespace ClassicLightFX.Core
{
    internal enum ClassicFeature
    {
        StockTables,
        SunGradient,
        SunPower,
        SunPosition,
        FogEffect,
        FogTint,
    }

    /// <summary>
    /// Reversible classic-style controls. The sun curve is transformed from
    /// the current map baseline and is not a historical gradient table.
    /// See PROCEDENCIA.md for the retained targets and remaining evidence limits.
    /// </summary>
    internal static class ClassicLook
    {
        private const float ClassicSunPower = 3.318695f;
        private const float ClassicSunExposure = 1f;

        internal static bool Active;
        private static readonly HashSet<ClassicFeature> Written = new HashSet<ClassicFeature>();
        internal static bool Owns(ClassicFeature feature) { return Active && Written.Contains(feature); }
        private static Baseline _baseline;
        private static ClassicFogDriver _fogDriver;
        private static GameObject _driverHost;
        private static bool _tablesSwapped;
        private static bool _warnedNotAttached;
        private static readonly Dictionary<string, Texture3DWrapper> ModernTableBackup = new Dictionary<string, Texture3DWrapper>();

        private sealed class Baseline
        {
            internal Gradient SunGradient;
            internal float SunPower;
            internal float SunExposure;
            internal float Latitude;
            internal float Longitude;
            internal Color SkyTint;
            internal Vector3 WaveLengths;
        }

        internal static bool Attached
        {
            get { return _baseline != null; }
        }

        internal static void Attach()
        {
            if (_baseline != null)
            {
                return;
            }

            var dayNight = Object.FindObjectOfType<DayNightProperties>();
            if (dayNight == null)
            {
                return;
            }

            _baseline = new Baseline
            {
                SunGradient = PropertyLedger.Baseline<Gradient>(dayNight, "m_LightColor"),
                SunPower = PropertyLedger.Baseline<float>(dayNight, "m_SunIntensity"),
                SunExposure = PropertyLedger.Baseline<float>(dayNight, "m_Exposure"),
                Latitude = PropertyLedger.Baseline<float>(dayNight, "m_Latitude"),
                Longitude = PropertyLedger.Baseline<float>(dayNight, "m_Longitude"),
                SkyTint = PropertyLedger.Baseline<Color>(dayNight, "m_SkyTint"),
                WaveLengths = PropertyLedger.Baseline<Vector3>(dayNight, "m_WaveLengths"),
            };
            _warnedNotAttached = false;
            Active = false;
            Written.Clear();

            if (_driverHost == null)
            {
                _driverHost = new GameObject("ClassicLightFX.FogDriver");
                _fogDriver = _driverHost.AddComponent<ClassicFogDriver>();

                // La referencia del tinte se toma aqui, con el resto y antes de aplicar nada,
                // no cuando al driver le toque su primer fotograma.
                _fogDriver.AdoptBaseline(_baseline.SkyTint, _baseline.WaveLengths);
            }
        }

        internal static void Detach()
        {
            Active = false;
            System.AppDomain.CurrentDomain.SetData("FX.ClassicRequests.v1", string.Empty);
            if (_baseline != null)
            {
                Apply(ClassicFeature.StockTables, false);
                Apply(ClassicFeature.SunGradient, false);
                Apply(ClassicFeature.SunPower, false);
                Apply(ClassicFeature.SunPosition, false);
                _baseline = null;
            }

            if (_fogDriver != null)
            {
                _fogDriver.Shutdown();
                _fogDriver = null;
            }

            if (_driverHost != null)
            {
                Object.Destroy(_driverHost);
                _driverHost = null;
            }

            PropertyLedger.ReleaseAll(); PropertyLedger.Forget();
            FxInterop.RefreshCompanions();
            _tablesSwapped = false;
            ModernTableBackup.Clear();
            ThemeOwnership.Forget();
        }

        internal static void Apply(ClassicFeature feature, bool classic)
        {
            if (_baseline == null)
            {
                if (!_warnedNotAttached)
                {
                    Debug.LogWarning("[ClassicLightFX] Feature change ignored: no map state captured yet.");
                    _warnedNotAttached = true;
                }

                return;
            }

            var dayNight = Object.FindObjectOfType<DayNightProperties>();
            if (dayNight == null)
            {
                return;
            }

            string field = feature == ClassicFeature.SunGradient ? "lightColor"
                : feature == ClassicFeature.SunPower ? "sunIntensity"
                : feature == ClassicFeature.SunPosition ? "sunPosition" : string.Empty;
            bool delegated = field.Length > 0 &&
                (Infrastructure.FxInterop.Claims("LumenFX.LumenFXMod", field)
                 || Infrastructure.FxInterop.Claims("SceneFX.SceneFXMod", field));
            if (delegated) { Written.Remove(feature); return; }
            if (feature != ClassicFeature.FogEffect && feature != ClassicFeature.FogTint)
            {
                if (!classic && !Written.Remove(feature)) return;
                if (classic) Written.Add(feature);
            }

            switch (feature)
            {
                case ClassicFeature.StockTables:
                    SwapTables(classic);
                    break;

                case ClassicFeature.SunGradient:
                    if (classic) PropertyLedger.Write(dayNight, "m_LightColor", BuildClassicSunCurve());
                    else PropertyLedger.Release(dayNight, "m_LightColor");
                    break;

                case ClassicFeature.SunPower:
                    if (classic) PropertyLedger.Write(dayNight, "m_SunIntensity", ClassicSunPower);
                    else PropertyLedger.Release(dayNight, "m_SunIntensity");

                    // Igual que arriba: aplicar lo clásico sí; devolver lo capturado, no.
                    if (!Infrastructure.FxInterop.Claims("LumenFX.LumenFXMod", "exposure")
                        && (classic || !ThemeOwnership.AtmosphereIsManaged))
                    {
                        if (classic) PropertyLedger.Write(dayNight, "m_Exposure", ClassicSunExposure);
                        else PropertyLedger.Release(dayNight, "m_Exposure");
                    }

                    break;

                case ClassicFeature.SunPosition:
                    ApplySunPosition(dayNight, classic);
                    break;

                case ClassicFeature.FogEffect:
                case ClassicFeature.FogTint:
                    if (_fogDriver != null)
                    {
                        _fogDriver.Refresh();
                    }

                    break;
            }
        }

        internal static void ApplyFromOptions()
        {
            var options = ModOptions.Instance;
            Active = !options.VanillaMode;
            if (!Active) { Release(); return; }
            PublishRequests();
            FxInterop.RefreshCompanions();
            Apply(ClassicFeature.StockTables, options.SwapLuts);
            Apply(ClassicFeature.SunGradient, options.SunColor);
            Apply(ClassicFeature.SunPower, options.SunStrength);
            Apply(ClassicFeature.SunPosition, options.SunCoords);
            Apply(ClassicFeature.FogEffect, options.ClassicFogMode);
            Apply(ClassicFeature.FogTint, options.ClassicFogTint);
        }

        internal static void PublishRequests()
        {
            var o = ModOptions.Instance;
            string requests = !Active || o.VanillaMode ? string.Empty
                : (o.SunColor ? "sunColor," : "") + (o.SunStrength ? "sunStrength," : "")
                + (o.SunCoords ? "sunCoords," : "") + (o.ClassicFogMode ? "fogMode," : "")
                + (o.ClassicFogWithCycle ? "fogWithCycle," : "");
            System.AppDomain.CurrentDomain.SetData("FX.ClassicRequests.v1", requests);
            float latitude, longitude;
            System.AppDomain.CurrentDomain.SetData("FX.ClassicCoordinates.v1", Active && o.SunCoords && TryGetCityCoordinates(LutLibrary.GetEnvironment(), out latitude, out longitude)
                ? new[] { latitude, longitude } : null);
        }

        private static void ApplySunPosition(DayNightProperties dayNight, bool classic)
        {
            // Poner las coordenadas clásicas es una petición expresa y se atiende. Lo que no
            // se hace, con un gestor de temas presente, es devolver las capturadas: se
            // capturaron antes de que el tema aplicara las suyas, y devolverlas lo borraría.

            if (!classic)
            {
                PropertyLedger.Release(dayNight, "m_Latitude");
                PropertyLedger.Release(dayNight, "m_Longitude");
                return;
            }

            float latitude;
            float longitude;
            if (TryGetCityCoordinates(LutLibrary.GetEnvironment(), out latitude, out longitude))
            {
                PropertyLedger.Write(dayNight, "m_Latitude", latitude);
                PropertyLedger.Write(dayNight, "m_Longitude", longitude);
            }
        }

        private static bool TryGetCityCoordinates(string environment, out float latitude, out float longitude)
        {
            switch (environment)
            {
                case "Europe":
                    latitude = 51.5072f;
                    longitude = -0.1275f;
                    return true;

                case "North":
                    latitude = 59.3293f;
                    longitude = 18.0686f;
                    return true;

                case "Sunny":
                    latitude = 35.8833f;
                    longitude = 14.5f;
                    return true;

                case "Tropical":
                    latitude = 21.4167f;
                    longitude = 39.8167f;
                    return true;

                default:
                    latitude = 0f;
                    longitude = 0f;
                    return false;
            }
        }

        internal static void Release()
        {
            Active = false;
            PublishRequests();
            FxInterop.RefreshCompanions();
            Apply(ClassicFeature.StockTables, false);
            Apply(ClassicFeature.SunGradient, false);
            Apply(ClassicFeature.SunPower, false);
            Apply(ClassicFeature.SunPosition, false);
            if (_fogDriver != null) _fogDriver.Shutdown();
            PropertyLedger.ReleaseAll();
        }

        private static Gradient BuildClassicSunCurve()
        {
            // Own neutral daylight approximation, sampled from this map's gradient.
            // No legacy palette or unverified eight-key arrangement is distributed.
            var dayNight = Object.FindObjectOfType<DayNightProperties>();
            var source = dayNight == null ? null : PropertyLedger.Baseline<Gradient>(dayNight, "m_LightColor");
            if (source == null) return null;
            var keys = source.colorKeys;
            for (int i = 0; i < keys.Length; i++)
            {
                float daylight = Mathf.Clamp01(1f - Mathf.Abs(keys[i].time - 0.5f) * 4f);
                keys[i] = new GradientColorKey(Color.Lerp(keys[i].color, Color.white, daylight * 0.35f), keys[i].time);
            }
            return new Gradient { colorKeys = keys, alphaKeys = source.alphaKeys };
        }

        private static void SwapTables(bool classic)
        {
            var manager = ColorCorrectionManager.instance;
            var renderProperties = Object.FindObjectOfType<RenderProperties>();
            if (manager == null || renderProperties == null)
            {
                return;
            }

            if (classic && Infrastructure.FxInterop.Claims("SceneFX.SceneFXMod", "lut")) { if (_tablesSwapped) SwapTables(false); return; }
            bool wantSwap = classic && !_tablesSwapped;
            bool wantRestore = !classic && _tablesSwapped;
            if (!wantSwap && !wantRestore)
            {
                return;
            }

            SwapBuiltinTables(manager, classic);
            SwapActiveTable(renderProperties, classic);
            ForcePipelineRebind(manager);
            _tablesSwapped = classic;
        }

        private static void SwapBuiltinTables(ColorCorrectionManager manager, bool toClassic)
        {
            if (manager.m_BuiltinLUTs == null)
            {
                return;
            }

            for (int i = 0; i < manager.m_BuiltinLUTs.Length; i++)
            {
                var current = manager.m_BuiltinLUTs[i];
                if (current == null)
                {
                    continue;
                }

                Texture3DWrapper replacement;
                if (toClassic && TryBuildClassicTable(current.name, out replacement))
                {
                    if (!ModernTableBackup.ContainsKey(current.name))
                    {
                        ModernTableBackup[current.name] = current;
                    }

                    manager.m_BuiltinLUTs[i] = replacement;
                }
                else if (!toClassic && ModernTableBackup.ContainsKey(current.name))
                {
                    manager.m_BuiltinLUTs[i] = ModernTableBackup[current.name];
                }
            }
        }

        private static void SwapActiveTable(RenderProperties renderProperties, bool toClassic)
        {
            var active = renderProperties.m_ColorCorrectionLUT;
            if (active == null)
            {
                return;
            }

            Texture3DWrapper replacement;
            if (toClassic && TryBuildClassicTable(active.name, out replacement))
            {
                if (!ModernTableBackup.ContainsKey(active.name))
                {
                    ModernTableBackup[active.name] = active;
                }

                renderProperties.m_ColorCorrectionLUT = replacement;
            }
            else if (!toClassic && ModernTableBackup.ContainsKey(active.name))
            {
                renderProperties.m_ColorCorrectionLUT = ModernTableBackup[active.name];
            }
        }

        private static bool TryBuildClassicTable(string builtinName, out Texture3DWrapper table)
        {
            if (!IsKnownTable(builtinName))
            {
                table = null;
                return false;
            }

            table = LutLibrary.Synthesize(builtinName);
            return true;
        }

        private static bool IsKnownTable(string builtinName)
        {
            return builtinName == "LUTeurope"
                || builtinName == "LUTSunny"
                || builtinName == "LUTNorth"
                || builtinName == "LUTTropical"
                || builtinName == "LUTWinter";
        }

        private static void ForcePipelineRebind(ColorCorrectionManager manager)
        {
            int count = manager.items != null ? manager.items.Length : 0;
            if (count == 0)
            {
                return;
            }

            int previous = manager.lastSelection;
            manager.currentSelection = (previous + 1) % count;
            manager.currentSelection = previous;
        }
    }
}
