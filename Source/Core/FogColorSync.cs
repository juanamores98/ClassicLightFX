using UnityEngine;
using ClassicLightFX.Options;

namespace ClassicLightFX.Core
{
    /// <summary>
    /// Keeps the After Dark fog color (sky tint and scattering wavelengths)
    /// matched to the classic values over the course of the day.
    /// </summary>
    public class FogColorSync : MonoBehaviour
    {
        private static readonly Color SkyTintClassic = new Color(0.40784313725490196f, 0.6509803921568627f, 0.8274509803921568f, 1.0f);
        private static readonly Vector3 WaveLengthsClassic = new Vector3(680f, 680f, 680f);

        private DayNightFogEffect _modernEffect;
        private DayNightProperties _dayNightProperties;

        private bool _cachedFogColorOption;
        private bool _cachedEffectEnabled;
        private Color _skyTintAd = Color.clear;
        private Vector3 _waveLengthsAd = Vector3.zero;
        private Color _cachedSkyTint;
        private Vector3 _cachedWaveLengths;
        private float _cachedTimeOfDay;

        private Gradient _skyTintGradient;
        private Gradient _waveLengthsGradient;

        public void Awake()
        {
            _modernEffect = Object.FindObjectOfType<DayNightFogEffect>();
            _dayNightProperties = Object.FindObjectOfType<DayNightProperties>();
            _waveLengthsAd = _dayNightProperties.m_WaveLengths;
            _skyTintAd = _dayNightProperties.m_SkyTint;

            _skyTintGradient = BuildStepGradient(new GradientColorKey(SkyTintClassic, 0.35f), new GradientColorKey(SkyTintClassic, 0.65f), _skyTintAd);
            _waveLengthsGradient = BuildStepGradient(
                new GradientColorKey(FromVector3(WaveLengthsClassic), 0.35f),
                new GradientColorKey(FromVector3(WaveLengthsClassic), 0.65f),
                FromVector3(_waveLengthsAd));
        }

        public void Update()
        {
            ReplaceFogColorIfNeeded();
        }

        public void OnDestroy()
        {
            if (_dayNightProperties != null)
            {
                ReplaceFogColorImpl(false);
            }
        }

        private static Gradient BuildStepGradient(GradientColorKey classicStart, GradientColorKey classicEnd, Color adColor)
        {
            return new Gradient
            {
                colorKeys = new[]
                {
                    new GradientColorKey(adColor, 0f),
                    new GradientColorKey(adColor, 0.29f),
                    classicStart,
                    classicEnd,
                    new GradientColorKey(adColor, 0.71f),
                    new GradientColorKey(adColor, 1f),
                },
                alphaKeys = new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 1f),
                },
            };
        }

        private void ReplaceFogColorIfNeeded()
        {
            if (_modernEffect == null ||
                (_cachedFogColorOption == ModOptions.Instance.ClassicFogTint &&
                 _cachedEffectEnabled == _modernEffect.enabled &&
                 _dayNightProperties.m_WaveLengths.Equals(_cachedWaveLengths) &&
                 _dayNightProperties.m_SkyTint.Equals(_cachedSkyTint) &&
                 _dayNightProperties.m_TimeOfDay.Equals(_cachedTimeOfDay)))
            {
                return;
            }

            ReplaceFogColorImpl(_modernEffect.enabled && ModOptions.Instance.ClassicFogTint);
            _cachedFogColorOption = ModOptions.Instance.ClassicFogTint;
            _cachedEffectEnabled = _modernEffect.enabled;
            _cachedWaveLengths = _dayNightProperties.m_WaveLengths;
            _cachedSkyTint = _dayNightProperties.m_SkyTint;
            _cachedTimeOfDay = _dayNightProperties.m_TimeOfDay;
        }

        private void ReplaceFogColorImpl(bool toClassic)
        {
            _dayNightProperties.m_SkyTint = toClassic
                ? _skyTintGradient.Evaluate(_dayNightProperties.normalizedTimeOfDay)
                : _skyTintAd;
            _dayNightProperties.m_WaveLengths = toClassic
                ? FromColor(_waveLengthsGradient.Evaluate(_dayNightProperties.normalizedTimeOfDay))
                : _waveLengthsAd;
        }

        private static Color FromVector3(Vector3 vector)
        {
            return new Color(vector.x / 1000f, vector.y / 1000f, vector.z / 1000f);
        }

        private static Vector3 FromColor(Color color)
        {
            return new Vector3(color.r * 1000f, color.g * 1000f, color.b * 1000f);
        }
    }
}
