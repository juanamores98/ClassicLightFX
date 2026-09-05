using UnityEngine;
using ColossalFramework;
using ClassicLightFX.Options;

namespace ClassicLightFX.Core
{
    /// <summary>
    /// Switches between the classic fog effect and the After Dark one,
    /// depending on the time of day and the mod options.
    /// </summary>
    public class FogEffectSync : MonoBehaviour
    {
        private bool _previousFogColorState;
        private bool _cachedNight;
        private bool _cachedDisableOption;
        private bool _cachedDayNightCycleState;

        private FogEffect _classicEffect;
        private DayNightFogEffect _modernEffect;

        public void Awake()
        {
            _classicEffect = Object.FindObjectOfType<FogEffect>();
            _modernEffect = Object.FindObjectOfType<DayNightFogEffect>();
            SetupEffectsIfNeeded(true);
        }

        public void Update()
        {
            SetupEffectsIfNeeded(false);
        }

        public void OnDestroy()
        {
            SetUpEffects(false);
        }

        private void SetupEffectsIfNeeded(bool forceSetup)
        {
            bool dayNightEnabled = Singleton<SimulationManager>.instance.m_enableDayNight;
            bool disableClassicIfDayNightOn = !ModOptions.Instance.ClassicFogMode;

            if (!forceSetup &&
                disableClassicIfDayNightOn == _cachedDisableOption &&
                dayNightEnabled == _cachedDayNightCycleState &&
                _cachedNight == SimulationManager.instance.m_isNightTime &&
                _previousFogColorState == ModOptions.Instance.ClassicFogTint)
            {
                return;
            }

            if (disableClassicIfDayNightOn && dayNightEnabled)
            {
                SetUpEffects(false);
            }
            else
            {
                SetUpEffects(!SimulationManager.instance.m_isNightTime);
            }

            _cachedDisableOption = disableClassicIfDayNightOn;
            _cachedNight = SimulationManager.instance.m_isNightTime;
            _cachedDayNightCycleState = dayNightEnabled;
            _previousFogColorState = ModOptions.Instance.ClassicFogTint;
        }

        private void SetUpEffects(bool toClassic)
        {
            if (_classicEffect == null || _modernEffect == null)
            {
                return;
            }

            _classicEffect.enabled = toClassic;
            _modernEffect.enabled = !toClassic;
        }
    }
}
