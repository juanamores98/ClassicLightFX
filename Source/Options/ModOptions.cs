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
        private static readonly string FileName = "ClassicLightFX2.xml";

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

        internal bool SwapLuts = true;
        internal bool SunColor = true;
        internal bool SunStrength = true;
        internal bool SunCoords = true;
        internal bool ClassicFogMode = true;
        internal bool ClassicFogTint = true;
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
                    if (serializer.Deserialize(reader) is OptionsDocument)
                    {
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
            _dirty = false;
            _lastSaveTime = Time.realtimeSinceStartup;
            try
            {
                using (var writer = new StreamWriter(OptionsPath))
                {
                    new XmlSerializer(typeof(OptionsDocument)).Serialize(writer, new OptionsDocument());
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        [XmlRoot(ElementName = "classicLightFx", Namespace = "", IsNullable = false)]
        public class OptionsDocument
        {
            [XmlAttribute("schema")]
            public int Schema = 2;

            [XmlElement("swapLuts")] public bool SwapLuts { get => ModOptions.Instance.SwapLuts; set => ModOptions.Instance.SwapLuts = value; }
            [XmlElement("sunColor")] public bool SunColor { get => ModOptions.Instance.SunColor; set => ModOptions.Instance.SunColor = value; }
            [XmlElement("sunStrength")] public bool SunStrength { get => ModOptions.Instance.SunStrength; set => ModOptions.Instance.SunStrength = value; }
            [XmlElement("sunCoords")] public bool SunCoords { get => ModOptions.Instance.SunCoords; set => ModOptions.Instance.SunCoords = value; }
            [XmlElement("fogMode")] public bool ClassicFogMode { get => ModOptions.Instance.ClassicFogMode; set => ModOptions.Instance.ClassicFogMode = value; }
            [XmlElement("fogTint")] public bool ClassicFogTint { get => ModOptions.Instance.ClassicFogTint; set => ModOptions.Instance.ClassicFogTint = value; }
            [XmlElement("applyOnLoad")] public bool ApplyOnLoad { get => ModOptions.Instance.ApplyOnLoad; set => ModOptions.Instance.ApplyOnLoad = value; }
            [XmlElement("windowX")] public float WindowX { get => ModOptions.Instance.WindowX; set => ModOptions.Instance.WindowX = value; }
            [XmlElement("windowY")] public float WindowY { get => ModOptions.Instance.WindowY; set => ModOptions.Instance.WindowY = value; }
        }
    }
}

