using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace ClassicLightFX.Options
{
    /// <summary>
    /// v2 option set. Element names and defaults belong to this version.
    /// </summary>
    internal sealed class ModOptions
    {
        private static readonly string FileName = "ClassicLightFX2.xml";

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
                if (!File.Exists(FileName))
                {
                    return;
                }

                using (var reader = new StreamReader(FileName))
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
                using (var writer = new StreamWriter(FileName))
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

