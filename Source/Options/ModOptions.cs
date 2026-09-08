using ColossalFramework.IO;
using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace ClassicLightFX.Options
{
    /// <summary>
    /// v2 option set. Element names and defaults belong to this version.
    /// </summary>
    /// <remarks>
    /// <b>Publica por obligacion, no por gusto.</b> XmlSerializer exige que el tipo que
    /// serializa y todos los que lo contienen sean publicos. Con esta clase en internal, su
    /// <see cref="OptionsDocument"/> anidado quedaba inaccesible para el serializador, y tanto
    /// Load como Save lanzaban en cada intento: el mod no llego nunca a guardar sus opciones.
    /// Se midio en partida —diecinueve excepciones en una sola tanda del lab— y no se habia
    /// visto antes porque las dos rutas se tragan la excepcion y siguen con los valores por
    /// defecto, que es exactamente lo que hace que el sintoma parezca otra cosa.
    /// </remarks>
    public sealed class ModOptions
    {
                /// <remarks>
        /// <b>Ruta completa, no relativa.</b> Un nombre suelto lo resuelve .NET contra el
        /// directorio de trabajo del proceso, que en Cities: Skylines es la carpeta de
        /// instalacion del juego. Ahi acababan estos XML: dentro de Archivos de Programa, donde
        /// escribir suele requerir permisos y donde una verificacion de Steam puede borrarlos.
        /// Se midio en partida —los cuatro archivos aparecieron en la carpeta del juego— y solo
        /// LumenFX lo hacia bien.
        ///
        /// <b>La migracion.</b> Si queda un archivo en el sitio antiguo y todavia no hay uno en
        /// el nuevo, se lee el antiguo: nadie pierde su configuracion por arreglar esto.
        /// </remarks>

        private static string OptionsPath
        {
            get { return Path.Combine(DataLocation.localApplicationData, "ClassicLightFX2.xml"); }
        }

        /// <summary>El sitio antiguo: la carpeta de trabajo del proceso.</summary>
        private static string OptionsPathLegacy
        {
            get { return "ClassicLightFX2.xml"; }
        }

        /// <summary>De donde leer: el sitio nuevo si existe, y si no el antiguo.</summary>
        private static string OptionsPathToRead
        {
            get
            {
                return File.Exists(OptionsPath) || !File.Exists(OptionsPathLegacy)
                    ? OptionsPath
                    : OptionsPathLegacy;
            }
        }

        internal static readonly ModOptions Instance = new ModOptions();

        internal bool SwapLuts;
        internal bool SunColor;
        internal bool SunStrength;
        internal bool SunCoords;
        internal bool ClassicFogMode;
        internal bool ClassicFogTint;
        /// <summary>
        /// Si el efecto de niebla clasico se mantiene con el ciclo dia/noche activo.
        /// </summary>
        /// <remarks>
        /// El original tenia este interruptor y aqui estaba fijo en "no": de noche y con ciclo,
        /// la niebla clasica se apagaba sola sin que nadie pudiera evitarlo. Era el unico
        /// ajuste de Daylight Classic que no tenia equivalente.
        /// </remarks>
        internal bool ClassicFogWithCycle;

        internal bool VanillaMode = true;
        internal bool ApplyOnLoad = true;
        internal float WindowX = 920f;
        internal float WindowY = 140f;

        private static float _lastSaveTime = -10f;
        private static bool _dirty;

        private ModOptions()
        {
        }

        internal static void Load()
        {
            try
            {
                if (!File.Exists(OptionsPathToRead))
                {
                    return;
                }

                using (var reader = new StreamReader(OptionsPathToRead))
                {
                    var serializer = new XmlSerializer(typeof(OptionsDocument));
                    if (serializer.Deserialize(reader) is OptionsDocument document)
                    {
                        document.Apply();
                        return;
                    }
                }

                Debug.Log("[ClassicLightFX v2] options file could not be deserialized");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        internal static void Save(bool immediate = false)
        {
            _dirty = true;
            float now = Time.realtimeSinceStartup;
            if (immediate || now - _lastSaveTime >= 1.0f)
            {
                SaveImmediate();
            }
        }

        internal static void CheckPendingSave()
        {
            if (_dirty && Time.realtimeSinceStartup - _lastSaveTime >= 1.0f)
            {
                SaveImmediate();
            }
        }

        internal static void SaveImmediate()
        {
            _dirty = true;
            _lastSaveTime = Time.realtimeSinceStartup;
            try
            {
                var document = new OptionsDocument { VanillaMode = Instance.VanillaMode };
                Infrastructure.FxStorage.WriteXml(OptionsPath, document);
                _dirty = false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        [XmlRoot(ElementName = "classicLightFx", Namespace = "", IsNullable = false)]
        public class OptionsDocument
        {
            public OptionsDocument() { VanillaMode = false; }

            [XmlAttribute("schema")]
            public int Schema = 2;

            private bool _SwapLuts = ModOptions.Instance.SwapLuts;
            [XmlElement("swapLuts")] public bool SwapLuts { get => _SwapLuts; set => _SwapLuts = value; }
            private bool _SunColor = ModOptions.Instance.SunColor;
            [XmlElement("sunColor")] public bool SunColor { get => _SunColor; set => _SunColor = value; }
            private bool _SunStrength = ModOptions.Instance.SunStrength;
            [XmlElement("sunStrength")] public bool SunStrength { get => _SunStrength; set => _SunStrength = value; }
            private bool _SunCoords = ModOptions.Instance.SunCoords;
            [XmlElement("sunCoords")] public bool SunCoords { get => _SunCoords; set => _SunCoords = value; }
            private bool _ClassicFogMode = ModOptions.Instance.ClassicFogMode;
            [XmlElement("fogMode")] public bool ClassicFogMode { get => _ClassicFogMode; set => _ClassicFogMode = value; }
            private bool _ClassicFogTint = ModOptions.Instance.ClassicFogTint;
            [XmlElement("fogTint")] public bool ClassicFogTint { get => _ClassicFogTint; set => _ClassicFogTint = value; }
            private bool _ClassicFogWithCycle = ModOptions.Instance.ClassicFogWithCycle;
            [XmlElement("classicFogWithCycle")] public bool ClassicFogWithCycle { get => _ClassicFogWithCycle; set => _ClassicFogWithCycle = value; }
            private bool _VanillaMode = ModOptions.Instance.VanillaMode;
            [XmlElement("vanillaMode")] public bool VanillaMode { get => _VanillaMode; set => _VanillaMode = value; }
            private bool _ApplyOnLoad = ModOptions.Instance.ApplyOnLoad;
            [XmlElement("applyOnLoad")] public bool ApplyOnLoad { get => _ApplyOnLoad; set => _ApplyOnLoad = value; }
            private float _WindowX = ModOptions.Instance.WindowX;
            [XmlElement("windowX")] public float WindowX { get => _WindowX; set => _WindowX = Infrastructure.FxStorage.Clamp(value, -100000f, 100000f); }
            private float _WindowY = ModOptions.Instance.WindowY;
            [XmlElement("windowY")] public float WindowY { get => _WindowY; set => _WindowY = Infrastructure.FxStorage.Clamp(value, -100000f, 100000f); }

        internal void Apply()
        {
            ModOptions.Instance.SwapLuts = SwapLuts;
            ModOptions.Instance.SunColor = SunColor;
            ModOptions.Instance.SunStrength = SunStrength;
            ModOptions.Instance.SunCoords = SunCoords;
            ModOptions.Instance.ClassicFogMode = ClassicFogMode;
            ModOptions.Instance.ClassicFogTint = ClassicFogTint;
            ModOptions.Instance.ClassicFogWithCycle = ClassicFogWithCycle;
            ModOptions.Instance.VanillaMode = VanillaMode;
            ModOptions.Instance.ApplyOnLoad = ApplyOnLoad;
            ModOptions.Instance.WindowX = WindowX;
            ModOptions.Instance.WindowY = WindowY;
        }

        }
    }
}

