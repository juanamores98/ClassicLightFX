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

        // Estado con el que se encontro cada componente de niebla. Al parar hay que devolver
        // esto y no una eleccion propia: AtmosphereFX tambien decide sobre los dos, y forzar
        // "el moderno encendido" le pisaria su configuracion al descargar el mapa.
        private bool _modernLayeredEnabled;
        private bool _modernLegacyEnabled;
        private bool _layeredStateCaptured;
        private bool _legacyStateCaptured;

        private bool _lastLegacyChoice;
        private bool _lastCycleState;
        private bool _lastNightState;
        private bool _lastTintActive;

        // Cierto solo si este driver llego a escribir el tinte alguna vez. Sin esto, apagar
        // el mod escribia su idea de "moderno" sobre lo que hubiera puesto el tema del mapa,
        // aunque el driver no hubiese tocado nada en toda la partida.
        private bool _tintEverApplied;
        private bool _fogEverApplied;

        /// <summary>
        /// Recibe el tinte atmosferico con el que se encontro el mapa.
        /// </summary>
        /// <remarks>
        /// <b>Por que se lo pasan y no lo toma el.</b> Antes lo capturaba en su primer
        /// <c>Update</c>, que ocurre un fotograma despues de que <c>ClassicLook</c> ya haya
        /// aplicado las opciones. Si para entonces el tinte clasico ya estaba puesto, el driver
        /// guardaba ese como "el moderno", y apagarlo despues no devolvia nada: reponia el mismo
        /// valor clasico. Se midio en partida: <c>m_SkyTint</c> se quedaba en 0.40784 aunque se
        /// apagaran todos los interruptores.
        ///
        /// Ahora la referencia se toma en <c>Attach</c>, junto al resto de la linea base y antes
        /// de que nadie escriba, y se le entrega aqui.
        /// </remarks>
        internal void AdoptBaseline(Color skyTint, Vector3 waveLengths)
        {
            _modernSkyTint = skyTint;
            _modernWavelengths = waveLengths;
            _tintCaptured = true;
        }

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
            RestoreFogComponents();
        }

        private void OnDestroy()
        {
            RestoreModernTint();
            RestoreFogComponents();
        }

        private void Resolve()
        {
            if (_layeredFog == null)
            {
                _layeredFog = Object.FindObjectOfType<DayNightFogEffect>();
            }

            if (_layeredFog != null && !_layeredStateCaptured)
            {
                _modernLayeredEnabled = _layeredFog.enabled;
                _layeredStateCaptured = true;
            }

            if (_legacyFog == null)
            {
                _legacyFog = Object.FindObjectOfType<FogEffect>();
            }

            if (_legacyFog != null && !_legacyStateCaptured)
            {
                _modernLegacyEnabled = _legacyFog.enabled;
                _legacyStateCaptured = true;
            }

            if (_dayNight == null)
            {
                _dayNight = Object.FindObjectOfType<DayNightProperties>();
            }

            // Si nadie le dio la referencia, la toma el mismo. Es el camino de respaldo:
            // el bueno es que se la pasen desde Attach, antes de que nada haya escrito.
            if (_dayNight != null && !_tintCaptured)
            {
                Debug.LogWarning("[ClassicLightFX] el driver de niebla capturo el tinte por su "
                    + "cuenta; el valor puede no ser el original del mapa.");
                _modernSkyTint = _dayNight.m_SkyTint;
                _modernWavelengths = _dayNight.m_WaveLengths;
                _tintCaptured = true;
            }
        }

        private void Update()
        {
            if (!ClassicLook.Active || ModOptions.Instance.VanillaMode) return;
            var simulation = Singleton<SimulationManager>.instance;
            if (simulation == null)
            {
                return;
            }

            Resolve();

            bool wantsClassic = ModOptions.Instance.ClassicFogMode;
            bool cycleEnabled = simulation.m_enableDayNight;
            bool night = simulation.m_isNightTime;

            // El original tenia un interruptor para esto y aqui estaba fijo: permitir o no el
            // efecto de niebla clasico cuando el ciclo dia/noche esta activo.
            bool allowWithCycle = ModOptions.Instance.ClassicFogWithCycle;
            bool useLegacy = wantsClassic && !night && (!cycleEnabled || allowWithCycle);
            if (!wantsClassic) RestoreFogComponents();
            else if (!_fogEverApplied || useLegacy != _lastLegacyChoice || cycleEnabled != _lastCycleState || night != _lastNightState)
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

            // El tinte clásico se aplica aunque haya gestor de temas: es una petición
            // expresa. Lo que no se hace es reponer el capturado al apagarlo —ver
            // RestoreModernTint—, porque se capturó antes de que el tema aplicara el suyo.
            if (_dayNight.m_SkyTint != targetTint)
            {
                Infrastructure.PropertyLedger.Write(_dayNight, "m_SkyTint", targetTint);
            }

            if (_dayNight.m_WaveLengths != targetWavelengths)
            {
                Infrastructure.PropertyLedger.Write(_dayNight, "m_WaveLengths", targetWavelengths);
            }

            _lastTintActive = true;
            _tintEverApplied = true;
        }

        private static float DaylightRamp(float normalizedDay)
        {
            float up = Mathf.Clamp01((normalizedDay - 0.29f) / 0.06f);
            float down = Mathf.Clamp01((0.71f - normalizedDay) / 0.06f);
            return Mathf.Min(up, down);
        }

        private void EnableLegacyFog(bool legacy)
        {
            if (Infrastructure.FxInterop.Claims("AtmosphereFX.AtmosphereFXMod", "fogEffect")) return;
            _fogEverApplied = true;
            if (_legacyFog != null)
            {
                Infrastructure.PropertyLedger.Write(_legacyFog, "enabled", legacy);
            }

            if (_layeredFog != null)
            {
                Infrastructure.PropertyLedger.Write(_layeredFog, "enabled", !legacy);
            }
        }

        /// <summary>Devuelve los dos componentes de niebla al estado en que se encontraron.</summary>
        private void RestoreFogComponents()
        {
            if (!_fogEverApplied) return;
            _fogEverApplied = false;
            if (Infrastructure.FxInterop.Claims("AtmosphereFX.AtmosphereFXMod", "fogEffect")) return;
            if (_legacyFog != null && _legacyStateCaptured)
            {
                Infrastructure.PropertyLedger.Release(_legacyFog, "enabled");
            }

            if (_layeredFog != null && _layeredStateCaptured)
            {
                Infrastructure.PropertyLedger.Release(_layeredFog, "enabled");
            }
        }

        private void RestoreModernTint()
        {
            if (_dayNight != null && _tintCaptured && _tintEverApplied)
            {
                if (!ThemeOwnership.AtmosphereIsManaged)
                {
                    Infrastructure.PropertyLedger.Release(_dayNight, "m_SkyTint");
                }

                Infrastructure.PropertyLedger.Release(_dayNight, "m_WaveLengths");
                _tintEverApplied = false;
            }
        }
    }
}
