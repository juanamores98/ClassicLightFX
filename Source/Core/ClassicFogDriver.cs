using UnityEngine;
using ColossalFramework;
using ClassicLightFX.Options;

namespace ClassicLightFX.Core
{
    /// <summary>
    /// Own per-frame driver for the classic fog behaviour: it selects between
    /// the game's two fog effect components according to the option state and
    /// the time of day, and it holds the atmospheric tint and scattering
    /// wavelengths on the pre-After Dark values while a classic look is
    /// active. Everything it touches is written back when it stops.
    /// </summary>
    public class ClassicFogDriver : MonoBehaviour
    {
        private static readonly Color ClassicSkyTint = new Color(104f / 255f, 166f / 255f, 211f / 255f, 1f);
        private static readonly Vector3 ClassicWavelengths = new Vector3(680f, 680f, 680f);

        private DayNightFogEffect _layeredFog;
        private FogEffect _legacyFog;
        private DayNightProperties _dayNight;

        private Color _modernSkyTint = Color.clear;
        private Vector3 _modernWavelengths = Vector3.zero;
        private bool _tintCaptured;

        private bool _lastLegacyChoice;
        private bool _lastCycleState;
        private bool _lastNightState;
        private bool _lastTintActive;

        public void Awake()
        {
            Resolve();
        }

        public void Refresh()
        {
            _lastLegacyChoice = !_lastLegacyChoice;
            _lastCycleState = !_lastCycleState;
            _lastNightState = !_lastNightState;
        }

        public void Shutdown()
        {
            RestoreModernTint();
            EnableLegacyFog(false);
        }

        private void OnDestroy()
        {
            RestoreModernTint();
            EnableLegacyFog(false);
        }

        private void Resolve()
        {
            if (_layeredFog == null)
            {
                _layeredFog = Object.FindObjectOfType<DayNightFogEffect>();
            }

            if (_legacyFog == null)
            {
                _legacyFog = Object.FindObjectOfType<FogEffect>();
            }

            if (_dayNight == null)
            {
                _dayNight = Object.FindObjectOfType<DayNightProperties>();
            }

            if (_dayNight != null && !_tintCaptured)
            {
                _modernSkyTint = _dayNight.m_SkyTint;
                _modernWavelengths = _dayNight.m_WaveLengths;
                _tintCaptured = true;
            }
        }

        private void Update()
        {
            var simulation = Singleton<SimulationManager>.instance;
            if (simulation == null)
            {
                return;
            }

            Resolve();

            bool wantsClassic = ModOptions.Instance.ClassicFogMode;
            bool cycleEnabled = simulation.m_enableDayNight;
            bool night = simulation.m_isNightTime;

            bool useLegacy = wantsClassic && (!cycleEnabled || !night);
            if (useLegacy != _lastLegacyChoice || cycleEnabled != _lastCycleState || night != _lastNightState)
            {
                EnableLegacyFog(useLegacy);
                _lastLegacyChoice = useLegacy;
                _lastCycleState = cycleEnabled;
                _lastNightState = night;
            }

            UpdateTint();
        }

        private void UpdateTint()
        {
            if (_dayNight == null || !_tintCaptured)
            {
                return;
            }

            bool tintActive = ModOptions.Instance.ClassicFogTint
                && _layeredFog != null
                && _layeredFog.enabled;

            if (!tintActive)
            {
                if (_lastTintActive)
                {
                    RestoreModernTint();
                }

                _lastTintActive = false;
                return;
            }

            float ramp = DaylightRamp(_dayNight.normalizedTimeOfDay);
            Color targetTint = Color.Lerp(_modernSkyTint, ClassicSkyTint, ramp);
            Vector3 targetWavelengths = Vector3.Lerp(_modernWavelengths, ClassicWavelengths, ramp);

            if (_dayNight.m_SkyTint != targetTint)
            {
                _dayNight.m_SkyTint = targetTint;
            }

            if (_dayNight.m_WaveLengths != targetWavelengths)
            {
                _dayNight.m_WaveLengths = targetWavelengths;
            }

            _lastTintActive = true;
        }

        private static float DaylightRamp(float normalizedDay)
        {
            float up = Mathf.Clamp01((normalizedDay - 0.29f) / 0.06f);
            float down = Mathf.Clamp01((0.71f - normalizedDay) / 0.06f);
            return Mathf.Min(up, down);
        }

        private void EnableLegacyFog(bool legacy)
        {
            if (_legacyFog != null)
            {
                _legacyFog.enabled = legacy;
            }

            if (_layeredFog != null)
            {
                _layeredFog.enabled = !legacy;
            }
        }

        private void RestoreModernTint()
        {
            if (_dayNight != null && _tintCaptured)
            {
                _dayNight.m_SkyTint = _modernSkyTint;
                _dayNight.m_WaveLengths = _modernWavelengths;
            }
        }
    }
}
