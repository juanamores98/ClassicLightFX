using UnityEngine;
using ColossalFramework;
using ClassicLightFX.Options;

namespace ClassicLightFX.Core
{
    /// <summary>
    /// Applies and reverts the classic (pre-After Dark) look: stock LUTs,
    /// sunlight color, intensity, sun position and fog effect selection.
    /// </summary>
    public static class ClassicTweaks
    {
        private const string Europe = "LUTeurope";
        private const string Sunny = "LUTSunny";
        private const string North = "LUTNorth";
        private const string Tropical = "LUTTropical";
        private const string Winter = "LUTWinter";

        private const float IntensityClassic = 3.318695f;
        private const float ExposureClassic = 1.0f;

        private static readonly Color32 NightKey = new Color32(55, 66, 77, byte.MaxValue);
        private static readonly Color32 SunriseKey = new Color32(245, 173, 84, byte.MaxValue);
        private static readonly Color32 MorningKey = new Color32(252, 222, 186, byte.MaxValue);
        private static readonly Color32 NoonKey = new Color32(255, 255, 255, byte.MaxValue);

        private static readonly Gradient ColorClassic = new Gradient
        {
            colorKeys = new[]
            {
                new GradientColorKey(NightKey, 0.23f),
                new GradientColorKey(SunriseKey, 0.26f),
                new GradientColorKey(MorningKey, 0.29f),
                new GradientColorKey(NoonKey, 0.35f),
                new GradientColorKey(NoonKey, 0.65f),
                new GradientColorKey(MorningKey, 0.71f),
                new GradientColorKey(SunriseKey, 0.74f),
                new GradientColorKey(NightKey, 0.77f),
            },
            alphaKeys = new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f),
            },
        };

        private static Texture3DWrapper _europeanClassic;
        private static Texture3DWrapper _sunnyClassic;
        private static Texture3DWrapper _northClassic;
        private static Texture3DWrapper _tropicalClassic;
        private static Texture3DWrapper _winterClassic;

        private static Texture3DWrapper _europeanAd;
        private static Texture3DWrapper _sunnyAd;
        private static Texture3DWrapper _northAd;
        private static Texture3DWrapper _tropicalAd;
        private static Texture3DWrapper _winterAd;

        private static float _intensityAd = -1.0f;
        private static float _exposureAd = -1.0f;
        private static float _lonAd = -1.0f;
        private static float _latAd = -1.0f;

        private static Gradient _colorAd;
        private static DayNightProperties _dayNightProperties;
        private static GameObject _gameObject;

        internal static void SetUp()
        {
            _dayNightProperties = Object.FindObjectOfType<DayNightProperties>();
            Object.FindObjectOfType<RenderProperties>().m_sun =
                _dayNightProperties.sunLightSource.transform; // fix sun position in some environments

            Reset();

            _gameObject = new GameObject("ClassicLightFX");
            _gameObject.AddComponent<FogColorSync>();

            ReplaceFogEffect(ModOptions.Instance.ClassicFogMode);
            ReplaceSunlightColor(ModOptions.Instance.SunColor);
            ReplaceSunlightIntensity(ModOptions.Instance.SunStrength);
            ReplaceLuts(ModOptions.Instance.SwapLuts);
            ReplaceLatLong(ModOptions.Instance.SunCoords);
        }

        internal static void CleanUp()
        {
            ReplaceFogEffect(false);
            ReplaceSunlightColor(false);
            ReplaceSunlightIntensity(false);
            ReplaceLuts(false);
            ReplaceLatLong(false);

            if (_gameObject != null)
            {
                Object.Destroy(_gameObject);
                _gameObject = null;
            }

            Reset();
            _dayNightProperties = null;
        }

        private static void Reset()
        {
            DestroyWrapper(ref _europeanClassic);
            DestroyWrapper(ref _tropicalClassic);
            DestroyWrapper(ref _northClassic);
            DestroyWrapper(ref _sunnyClassic);
            DestroyWrapper(ref _winterClassic);

            _europeanAd = null;
            _tropicalAd = null;
            _northAd = null;
            _sunnyAd = null;
            _winterAd = null;
            _intensityAd = -1.0f;
            _lonAd = -1.0f;
            _latAd = -1.0f;
        }

        private static void DestroyWrapper(ref Texture3DWrapper wrapper)
        {
            if (wrapper != null)
            {
                Object.Destroy(wrapper);
                wrapper = null;
            }
        }

        internal static void ReplaceLuts(bool toClassic)
        {
            if (!InGame)
            {
                return;
            }

            var manager = ColorCorrectionManager.instance;
            for (var i = 0; i < manager.m_BuiltinLUTs.Length; i++)
            {
                var replacement = GetReplacementLut(toClassic, manager.m_BuiltinLUTs[i].name, manager.m_BuiltinLUTs[i]);
                if (replacement == null)
                {
                    continue;
                }

                manager.m_BuiltinLUTs[i] = replacement;
            }

            var renderProperties = Object.FindObjectOfType<RenderProperties>();
            var replacement2 = GetReplacementLut(toClassic, renderProperties.m_ColorCorrectionLUT.name, renderProperties.m_ColorCorrectionLUT);
            if (replacement2 != null)
            {
                renderProperties.m_ColorCorrectionLUT = replacement2;
            }

            // Force the color correction pipeline to rebind the swapped LUTs.
            int size = manager.items.Length;
            int lastSelection = manager.lastSelection;
            manager.currentSelection = (lastSelection + 1) % size;
            manager.currentSelection = lastSelection;
        }

        private static Texture3DWrapper GetReplacementLut(bool toClassic, string builtinLutName, Texture3DWrapper builtinLut)
        {
            switch (builtinLutName)
            {
                case Europe:
                    if (_europeanAd == null) _europeanAd = builtinLut;
                    if (_europeanClassic == null) _europeanClassic = LutLibrary.Load("ClassicLightFX.LUTs.EuropeanClassic.png", Europe);
                    return toClassic ? _europeanClassic : _europeanAd;

                case Tropical:
                    if (_tropicalAd == null) _tropicalAd = builtinLut;
                    if (_tropicalClassic == null) _tropicalClassic = LutLibrary.Load("ClassicLightFX.LUTs.TropicalClassic.png", Tropical);
                    return toClassic ? _tropicalClassic : _tropicalAd;

                case North:
                    if (_northAd == null) _northAd = builtinLut;
                    if (_northClassic == null) _northClassic = LutLibrary.Load("ClassicLightFX.LUTs.BorealClassic.png", North);
                    return toClassic ? _northClassic : _northAd;

                case Sunny:
                    if (_sunnyAd == null) _sunnyAd = builtinLut;
                    if (_sunnyClassic == null) _sunnyClassic = LutLibrary.Load("ClassicLightFX.LUTs.TemperateClassic.png", Sunny);
                    return toClassic ? _sunnyClassic : _sunnyAd;

                case Winter:
                    if (_winterAd == null) _winterAd = builtinLut;
                    if (_winterClassic == null) _winterClassic = LutLibrary.Load("ClassicLightFX.LUTs.WinterClassic.png", Winter);
                    return toClassic ? _winterClassic : _winterAd;

                default:
                    return null;
            }
        }

        internal static void ReplaceSunlightIntensity(bool toClassic)
        {
            if (!InGame)
            {
                return;
            }

            if (_intensityAd < 0f)
            {
                _intensityAd = _dayNightProperties.m_SunIntensity;
            }

            if (_exposureAd < 0f)
            {
                _exposureAd = _dayNightProperties.m_Exposure;
            }

            _dayNightProperties.m_SunIntensity = toClassic ? IntensityClassic : _intensityAd;
            _dayNightProperties.m_Exposure = toClassic ? ExposureClassic : _exposureAd;
        }

        internal static void ReplaceSunlightColor(bool toClassic)
        {
            if (!InGame)
            {
                return;
            }

            if (_colorAd == null)
            {
                _colorAd = _dayNightProperties.m_LightColor;
            }

            _dayNightProperties.m_LightColor = toClassic ? ColorClassic : _colorAd;
        }

        internal static void ReplaceFogEffect(bool toClassic)
        {
            if (!InGame)
            {
                return;
            }

            if (toClassic)
            {
                if (_gameObject.GetComponent<FogEffectSync>() == null)
                {
                    _gameObject.AddComponent<FogEffectSync>();
                }
            }
            else
            {
                if (_gameObject.GetComponent<FogEffectSync>() != null)
                {
                    Object.Destroy(_gameObject.GetComponent<FogEffectSync>());
                }
            }
        }

        internal static void ReplaceLatLong(bool toClassic)
        {
            if (!InGame)
            {
                return;
            }

            if (_lonAd < 0.0f)
            {
                _lonAd = _dayNightProperties.m_Longitude;
            }

            if (_latAd < 0.0f)
            {
                _latAd = _dayNightProperties.m_Latitude;
            }

            if (toClassic)
            {
                switch (LutLibrary.GetEnvironment())
                {
                    case "Europe": // London
                        _dayNightProperties.m_Latitude = 51.5072f;
                        _dayNightProperties.m_Longitude = -0.1275f;
                        break;

                    case "North": // Stockholm
                        _dayNightProperties.m_Latitude = 59.3293f;
                        _dayNightProperties.m_Longitude = 18.0686f;
                        break;

                    case "Sunny": // Malta
                        _dayNightProperties.m_Latitude = 35.8833f;
                        _dayNightProperties.m_Longitude = 14.5000f;
                        break;

                    case "Tropical": // Mecca
                        _dayNightProperties.m_Latitude = 21.4167f;
                        _dayNightProperties.m_Longitude = 39.8167f;
                        break;
                }
            }
            else
            {
                _dayNightProperties.m_Latitude = _latAd;
                _dayNightProperties.m_Longitude = _lonAd;
            }
        }

        private static bool InGame
        {
            get { return _gameObject != null; }
        }
    }
}
