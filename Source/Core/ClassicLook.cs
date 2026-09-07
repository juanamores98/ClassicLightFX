using System.Collections.Generic;
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
    /// Own implementation of the classic (pre-After Dark) look. On attach it
    /// records whatever the game currently uses; each feature can then be
    /// switched between the modern values and the values the pre-After Dark
    /// release shipped with. Those numeric targets (sun power, sun curve,
    /// city coordinates, fog tint and wavelengths) are properties of the
    /// game itself; the code and structure around them are original to v2.
    /// </summary>
    internal static class ClassicLook
    {
        private const float ClassicSunPower = 3.318695f;
        private const float ClassicSunExposure = 1f;

        private static readonly Color32 NightSunColor = new Color32(55, 66, 77, 255);
        private static readonly Color32 DawnSunColor = new Color32(245, 173, 84, 255);
        private static readonly Color32 MorningSunColor = new Color32(252, 222, 186, 255);
        private static readonly Color32 DaySunColor = new Color32(255, 255, 255, 255);

        private static readonly float[] SunCurveTimes = { 0.23f, 0.26f, 0.29f, 0.35f, 0.65f, 0.71f, 0.74f, 0.77f };
        private static readonly Color32[] SunCurveColors =
        {
            NightSunColor, DawnSunColor, MorningSunColor, DaySunColor,
            DaySunColor, MorningSunColor, DawnSunColor, NightSunColor,
        };

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
                SunGradient = dayNight.m_LightColor,
                SunPower = dayNight.m_SunIntensity,
                SunExposure = dayNight.m_Exposure,
                Latitude = dayNight.m_Latitude,
                Longitude = dayNight.m_Longitude,
                SkyTint = dayNight.m_SkyTint,
                WaveLengths = dayNight.m_WaveLengths,
            };
            _warnedNotAttached = false;

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

            switch (feature)
            {
                case ClassicFeature.StockTables:
                    SwapTables(classic);
                    break;

                case ClassicFeature.SunGradient:
                    dayNight.m_LightColor = classic ? BuildClassicSunCurve() : _baseline.SunGradient;
                    break;

                case ClassicFeature.SunPower:
                    dayNight.m_SunIntensity = classic ? ClassicSunPower : _baseline.SunPower;

                    // Igual que arriba: aplicar lo clásico sí; devolver lo capturado, no.
                    if (classic || !ThemeOwnership.AtmosphereIsManaged)
                    {
                        dayNight.m_Exposure = classic ? ClassicSunExposure : _baseline.SunExposure;
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
            Apply(ClassicFeature.StockTables, options.SwapLuts);
            Apply(ClassicFeature.SunGradient, options.SunColor);
            Apply(ClassicFeature.SunPower, options.SunStrength);
            Apply(ClassicFeature.SunPosition, options.SunCoords);
            Apply(ClassicFeature.FogEffect, options.ClassicFogMode);
            Apply(ClassicFeature.FogTint, options.ClassicFogTint);
        }

        private static void ApplySunPosition(DayNightProperties dayNight, bool classic)
        {
            // Poner las coordenadas clásicas es una petición expresa y se atiende. Lo que no
            // se hace, con un gestor de temas presente, es devolver las capturadas: se
            // capturaron antes de que el tema aplicara las suyas, y devolverlas lo borraría.
            if (!classic && ThemeOwnership.AtmosphereIsManaged)
            {
                return;
            }

            if (!classic)
            {
                dayNight.m_Latitude = _baseline.Latitude;
                dayNight.m_Longitude = _baseline.Longitude;
                return;
            }

            float latitude;
            float longitude;
            if (TryGetCityCoordinates(LutLibrary.GetEnvironment(), out latitude, out longitude))
            {
                dayNight.m_Latitude = latitude;
                dayNight.m_Longitude = longitude;
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

        private static Gradient BuildClassicSunCurve()
        {
            var keys = new GradientColorKey[SunCurveTimes.Length];
            for (int i = 0; i < SunCurveTimes.Length; i++)
            {
                keys[i] = new GradientColorKey(SunCurveColors[i], SunCurveTimes[i]);
            }

            return new Gradient
            {
                colorKeys = keys,
                alphaKeys = new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 1f),
                },
            };
        }

        private static void SwapTables(bool classic)
        {
            var manager = ColorCorrectionManager.instance;
            var renderProperties = Object.FindObjectOfType<RenderProperties>();
            if (manager == null || renderProperties == null)
            {
                return;
            }

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
