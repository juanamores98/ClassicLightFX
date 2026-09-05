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

        internal static void Save()
        {
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
        }
    }
}
